using ConstructionMaterialOrderingApi.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.WarehouseProductStatusReportDto;
using ConstructionMaterialOrderingApi.Repositories;
using System.Security.Claims;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseProductStatusReportController : ControllerBase
    {
        private readonly IProductStatusReport _productStatusReport;
        private readonly IWareHouseAdminRepository _wareHouseAdminRepository;
        private readonly IHardwareStoreUserRepository _storeUserRepository;

        public WarehouseProductStatusReportController(IProductStatusReport productStatusReport,
            IWareHouseAdminRepository wareHouseAdminRepository, IHardwareStoreUserRepository storeUserRepository)
        {
            _productStatusReport = productStatusReport;
            _wareHouseAdminRepository = wareHouseAdminRepository;
            _storeUserRepository = storeUserRepository;
        } 
        [HttpPost]
        [Route("add-report")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> AddReport([FromBody] AddProductStatusReportDto statusReportDto)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdminInfo = await _wareHouseAdminRepository.GetWarehouseAdminByAccountId(userAppId);
            (bool isAdded, string message) = await _productStatusReport
                .Add(statusReportDto, warehouseAdminInfo.HardwareStoreId, warehouseAdminInfo.WarehouseId);
            return isAdded ? Ok(new { Success = 1, Message = "Report added successfully." }) :
                BadRequest(new { Success = 0, Message = message});
        }
        [HttpPost]
        [Route("add-status-report/{warehouseReportId}")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> AddProductStatusReport([FromBody] ProductStatusReportDto statusReportDto, int warehouseReportId)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdminInfo = await _wareHouseAdminRepository.GetWarehouseAdminByAccountId(userAppId);
            await _productStatusReport.AddProductStatusReport(warehouseReportId, statusReportDto);
            return Ok(new { Success = 1, Message = "Added Successfully."});
        }

        [HttpGet]
        [Route("get-reports")]
        [Authorize(Roles = "SuperAdmin,StoreOwner,WarehouseAdmin")]
        public async Task<IActionResult> GetReports()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _storeUserRepository.GetUserByAccountId(userAppId);
            var reports = await _productStatusReport.GetReports(hardwareStoreUserInfo.HardwareStoreId);

            var jsonResult = ConvertToJson(reports);
            return Ok(jsonResult);
        } 

        [HttpGet]
        [Route("get-report/{warehouseReportId}")]
        [Authorize(Roles = "SuperAdmin,StoreOwner,WarehouseAdmin")]
        public async Task<IActionResult> GetReport(int warehouseReportId)
        {
            var report = await _productStatusReport.GetReport(warehouseReportId);
            return Ok(ConvertToJson(report));
        }

        [HttpDelete]
        [Route("delete-report/{warehouseReportId}")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> RemoveReport(int warehouseReportId)
        {
            await _productStatusReport.RemoveReport(warehouseReportId);
            return Ok(new { Success = 1, Message = "Successfully deleted."});
        }

        [HttpGet]
        [Route("get-status-reports/{warehouseReportId}")]
        [Authorize(Roles = "SuperAdmin,StoreOwner,WarehouseAdmin")]
        public async Task<IActionResult> GetStatusReports(int warehouseReportId)
        {
            var statusReports = await _productStatusReport.GetProductStatusReports(warehouseReportId);
            return Ok(ConvertToJson(statusReports));
        } 
        [HttpDelete]
        [Route("delete-status-report/{productStatusReportId}")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> RemoveStatusReport(int productStatusReportId)
        {
            await _productStatusReport.RemoveStatusReport(productStatusReportId);
            return Ok(new { Success = 1, Message = "Deleted Successfully."});
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
