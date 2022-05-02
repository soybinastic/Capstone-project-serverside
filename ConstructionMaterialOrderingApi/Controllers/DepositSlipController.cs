using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ConstructionMaterialOrderingApi.Dtos.DepositSlipDtos;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositSlipController : ControllerBase
    {
        private readonly IStoreAdminRepository _storeAdminRepository;
        private readonly IDepositSlipRepository _depositSlipRepository;

        public DepositSlipController(IStoreAdminRepository storeAdminRepository, IDepositSlipRepository depositSlipRepository)
        {
            _storeAdminRepository = storeAdminRepository;
            _depositSlipRepository = depositSlipRepository;
        }

        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> Add([FromBody]DepositSlipDto depositSlipDto)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);

            await _depositSlipRepository.Add(depositSlipDto, storeAdmin.BranchId);
            return Ok(new { Success = 1, Message = "Successfully Saved"});
        }
        [HttpGet]
        [Route("get/{depositSlipId}")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetDepositSlip([FromRoute]int depositSlipId)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);

            var depositSlip = await _depositSlipRepository.GetDepositSlip(storeAdmin.BranchId, depositSlipId);
            return Ok(ConvertToJson(depositSlip));
        }

        [HttpGet]
        [Route("get")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetDepositSlips()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);

            var depositSlips = await _depositSlipRepository.GetDepositSlips(storeAdmin.BranchId);
            return Ok(ConvertToJson(depositSlips));
        }

        [HttpPut]
        [Route("update/{depositSlipId}")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> Update([FromRoute]int depositSlipId, [FromBody]DepositSlipDto depositSlipDto)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);

            var result = await _depositSlipRepository.Update(storeAdmin.BranchId, depositSlipId, depositSlipDto);
            return result ? Ok(new { Success = 1, Message = "Successfully Modified"}) : BadRequest();
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
