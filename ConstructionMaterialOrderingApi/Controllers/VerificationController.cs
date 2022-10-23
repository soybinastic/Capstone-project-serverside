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
using ConstructionMaterialOrderingApi.Context;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationRepository _verificationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ApplicationDbContext _db;
        public VerificationController(IVerificationRepository verificationRepository,
            ICustomerRepository customerRepository,
            ApplicationDbContext db)
        {
            _verificationRepository = verificationRepository;
            _customerRepository = customerRepository;
            _db = db;
        }
        [HttpPost]
        [Route("post")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PostVerificationData([FromForm] VerificationDetail verificationDetail)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(userAppId);
            (bool result, string message) = await _verificationRepository.Post(verificationDetail, customer.CustomerId);

            return result ? Ok(new { Success = 1, Message = message }) : BadRequest(new { Success = 0, Message = message });
        }
        [HttpGet]
        [Route("get/{id}")]
        [Authorize(Roles = "Admin,UserValidator")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var verificationDetail = await _verificationRepository.Get(id);
            return Ok(ConvertToJson(verificationDetail));
        }

        [HttpGet]
        [Route("get")]
        [Authorize(Roles = "Admin,UserValidator")]
        public async Task<IActionResult> GetAll()
        {
            var verificationDetails = await _verificationRepository.GetAll();
            return Ok(ConvertToJson(verificationDetails));
        }

        [HttpPut]
        [Route("verify-customer/{customerId}")]
        [Authorize(Roles = "Admin,UserValidator")]
        public async Task<IActionResult> VerifyCustomer([FromRoute] int customerId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            (bool result, string message) = await _verificationRepository.VerifyCustomer(customerId, userId);
            return result ? Ok(new { Success = 1, Message = message }) : BadRequest(new { Success = 0, Message = message });
        }

        [HttpGet("verified-users")]
        [Authorize(Roles = "Admin,UserValidator")]
        public async Task<IActionResult> GetVerifiedUsers()
        {
            var verifiedUsers = await _db.VerifiedUsers
                .Select(vu => new { Customer = vu.Customer, User = _db.Users.FirstOrDefault(u => u.Id == vu.ConfirmedBy) })
                .ToListAsync();
            return Ok(ConvertToJson(verifiedUsers));
        }

        [HttpGet("registered-companies")]
        [Authorize(Roles = "Admin,CompanyValidator")]
        public async Task<IActionResult> GetResgisteredCompanies()
        {
            var registeredCompanies = await _db.RegisteredCompanies
                .Select(rc => new { HardwareStore = rc.HardwareStore, User = _db.Users.FirstOrDefault(u => u.Id == rc.RegisteredBy), RegisteredDate = rc.DateConfirmed })
                .ToListAsync();
            return Ok(ConvertToJson(registeredCompanies));
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
