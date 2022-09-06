using System;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IPaymentRepository<Session> _paymentRepsitory;
        public PaymentController(IDashboardRepository dashboardRepository, IPaymentRepository<Session> paymentRepsitory)
        {
            _dashboardRepository = dashboardRepository;
            _paymentRepsitory = paymentRepsitory;
        }

        [HttpPost("checkout/{dashboardId:int}")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> Checkout([FromRoute]int dashboardId)
        {
            string successUrl = HttpContext.Request.Query["success"].ToString();
            string cancelUrl = HttpContext.Request.Query["cancel"].ToString();

            var dashboard = await _dashboardRepository.GetById(dashboardId);
            if(dashboard == null || dashboard.Status == Keyword.PAID || dashboard.Status == Keyword.ON_GOING)
            {
                return BadRequest(new { Success = 0, Message = "Failed to pay." });
            }

            var session = await _paymentRepsitory.OnPay(dashboard, successUrl, cancelUrl);
            return Ok(new { Url = session.Url });
        }
    }
}