using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Implementations;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ConstructionMaterialOrderingApi.Dtos.DeliverProductDtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliverProductController : ControllerBase
    {
        private readonly IBranchProductFactory _branchProductFactory;
        private readonly IDeliverProductReportRepository _deliverProductReportRepository;
        private readonly IWareHouseAdminRepository _wareHouseAdminRepository;

        public DeliverProductController(IBranchProductFactory branchProductFactory, 
            IDeliverProductReportRepository deliverProductReportRepository,
            IWareHouseAdminRepository wareHouseAdminRepository)
        {
            _branchProductFactory = branchProductFactory;
            _deliverProductReportRepository = deliverProductReportRepository;
            _wareHouseAdminRepository = wareHouseAdminRepository;
        } 

        [HttpPost]
        [Route("deliver-product")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> DeliverProduct([FromBody]DeliverProductDto deliverProductDto)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdminInfo = await _wareHouseAdminRepository.GetWarehouseAdminByAccountId(userAppId);

            var isSucceed = await _branchProductFactory.DeliverProduct(warehouseAdminInfo.WarehouseId, warehouseAdminInfo.HardwareStoreId, deliverProductDto);
            return isSucceed ? Ok(new { Success = 1, Message = "Successfully delivered."}) : BadRequest(new { Success = 0, Message = ""});
        } 

        [HttpGet]
        [Route("get-reports/{warehouseId}")]
        [Authorize(Roles = "WarehouseAdmin,SuperAdmin,StoreOwner")]
        public async Task<IActionResult> GetDeliverProductReports(int warehouseId)
        {
            var reports = await _deliverProductReportRepository.GetDeliverProducts(warehouseId);
            return Ok(ConvertToJson(reports));
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
