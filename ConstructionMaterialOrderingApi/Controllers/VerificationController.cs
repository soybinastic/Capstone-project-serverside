using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using ConstructionMaterialOrderingApi.Dtos.CustomerVerificationDtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationRepository _verificationRepository;
        private readonly ICustomerRepository _customerRepository;
        public VerificationController(IVerificationRepository verificationRepository, ICustomerRepository customerRepository)
        {
            _verificationRepository = verificationRepository;
            _customerRepository = customerRepository;
        }
        [HttpPost]
        [Route("post")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PostVerificationData([FromForm]VerificationDetail verificationDetail)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(userAppId);
            (bool result, string message) = await _verificationRepository.Post(verificationDetail, customer.CustomerId);

            return result ? Ok(new { Success = 1, Message = message}) : BadRequest(new { Success = 0, Message = message});
        }
        [HttpGet]
        [Route("get/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            var verificationDetail = await _verificationRepository.Get(id);
            return Ok(ConvertToJson(verificationDetail));
        }

        [HttpGet]
        [Route("get")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var verificationDetails = await _verificationRepository.GetAll();
            return Ok(ConvertToJson(verificationDetails));
        }

        [HttpPut]
        [Route("verify-customer/{customerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyCustomer([FromRoute]int customerId)
        {
            (bool result, string message) = await _verificationRepository.VerifyCustomer(customerId);
            return result ? Ok(new { Success = 1, Message = message}) : BadRequest(new { Success = 0, Message = message});
        }
        private string ConvertToJson<T>(T obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return jsonObj;
        }
    }
}
