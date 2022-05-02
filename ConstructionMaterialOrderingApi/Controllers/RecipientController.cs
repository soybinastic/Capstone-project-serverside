using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipientController : ControllerBase
    {
        private readonly IRecipientRepository _recipientRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStoreAdminRepository _storeAdminRepository;

        public RecipientController(IRecipientRepository recipientRepository, ICustomerRepository customerRepository, 
            IStoreAdminRepository storeAdminRepository)
        {
            _recipientRepository = recipientRepository;
            _customerRepository = customerRepository;
            _storeAdminRepository = storeAdminRepository;
        } 

        [HttpGet]
        [Route("customer")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAllRecipientsByCustomer()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(userAppId);
            var recipients = await _recipientRepository.GetAlRecipientsByCustomerId(customer.CustomerId);
            return Ok(ConvertToJson(recipients));
        }
        [HttpGet]
        [Route("customer/{recipientId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetRecipientByCustomer([FromRoute]int recipientId)
        {
            var recipient = await _recipientRepository.GetRecipient(recipientId);
            return Ok(ConvertToJson(recipient));
        }

        [HttpGet]
        [Route("get/{orderId}")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetRecipientByOrderId([FromRoute]int orderId)
        {
            //var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var recipient = await _recipientRepository.GetRecipientByOrderId(orderId);
            return Ok(ConvertToJson(recipient));
        } 

        [HttpGet]
        [Route("get")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetAllRecipients()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var recipients = await _recipientRepository.GetAllRecipientsByBranchId(storeAdmin.BranchId);
            return Ok(ConvertToJson(recipients));
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
