using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using ConstructionMaterialOrderingApi.Dtos.RequestDtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IStoreAdminRepository _storeAdminRepository;
        private readonly IWareHouseAdminRepository _wareHouseAdminRepository;
        private readonly IRequestRepository _requestRepository;

        public RequestController(IStoreAdminRepository storeAdminRepository, 
            IWareHouseAdminRepository wareHouseAdminRepository, 
            IRequestRepository requestRepository)
        {
            _storeAdminRepository = storeAdminRepository;
            _wareHouseAdminRepository = wareHouseAdminRepository;
            _requestRepository = requestRepository;
        } 
        [HttpPost]
        [Route("send-requestproduct")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> SendRequest([FromBody] RequestProductDto requestProductDto)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdminInfo = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);

            (bool isSucceed,string message) = await _requestRepository.Send(requestProductDto, storeAdminInfo.BranchId);
            return isSucceed ? Ok(new { Success = 1, Message = message}) : BadRequest(new { Success = 0, Message = message});
        }

        [HttpGet]
        [Route("get-branch-requests")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetBranchRequests()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdminInfo = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var requests = await _requestRepository.GetAllRequestsByBranchId(storeAdminInfo.BranchId);

            return Ok(ConvertToJson(requests));
        }

        [HttpGet]
        [Route("get-requests-send")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> GetRequestsSend()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdminInfo = await _wareHouseAdminRepository.GetWarehouseAdminByAccountId(userAppId);
            var requests = await _requestRepository.GetAllRequestsByWarehouseId(warehouseAdminInfo.WarehouseId);
            return Ok(ConvertToJson(requests));
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
