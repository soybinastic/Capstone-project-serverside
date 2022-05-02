using ConstructionMaterialOrderingApi.Dtos.RecieveProductDtos;
using ConstructionMaterialOrderingApi.Implementations;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecieveProductReportController : ControllerBase
    {
        private readonly IRecieveProductReport _recieveProductReport;
        private readonly IHardwareStoreUserRepository _hardwareStoreUserRepository;
        private readonly IWareHouseAdminRepository _wareHouseAdminRepository;

        public RecieveProductReportController(IRecieveProductReport recieveProductReport, 
            IHardwareStoreUserRepository hardwareStoreUserRepository, IWareHouseAdminRepository wareHouseAdminRepository)
        {
            _recieveProductReport = recieveProductReport;
            _hardwareStoreUserRepository = hardwareStoreUserRepository;
            _wareHouseAdminRepository = wareHouseAdminRepository;
        } 

        [HttpPost]
        [Route("add-recieveproduct-report")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> AddReport([FromBody] AddRecieveProductDto recieveProductDto)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdminInfo = await _wareHouseAdminRepository.GetWarehouseAdminByAccountId(userAppId);

            (bool isSucceed, string message) = await _recieveProductReport.Add(recieveProductDto, warehouseAdminInfo.HardwareStoreId, warehouseAdminInfo.WarehouseId);
            return isSucceed ? Ok(new { Success = 1, Message = message != "" ? message : "Added successfully." }) : BadRequest(new { Success = 0, Message = message});
        } 
        [HttpPost]
        [Route("add-recieveproduct/{warehouseReportId}")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> AddRecieveProduct([FromBody] RecieveProductDto recieveProductDto, 
            [FromRoute] int warehouseReportId)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdminInfo = await _wareHouseAdminRepository.GetWarehouseAdminByAccountId(userAppId);
            var isSucceed = await _recieveProductReport.AddRecieveProduct(recieveProductDto, warehouseReportId, warehouseAdminInfo.WarehouseId);
            return isSucceed ? Ok(new { Success = 1, Message = "Inserted successfully." }) : BadRequest(new { Success = 0, Message = "Failed to insert."});
        }
        [HttpPut]
        [Route("update-recieveproduct/{recieveProductId}")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> UpdateRecieveProduct([FromBody] RecieveProductDto recieveProductDto, 
            [FromRoute] int recieveProductId)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdminInfo = await _wareHouseAdminRepository.GetWarehouseAdminByAccountId(userAppId);
            var isSucceed = await _recieveProductReport.UpdateRecieveProduct(recieveProductDto, recieveProductId, warehouseAdminInfo.WarehouseId);
            return isSucceed ? Ok(new { Success = 1, Message = "Updated successfully." }) : BadRequest(new { Success = 0, Message = "Something went wrong." });
        } 
        [HttpGet]
        [Route("get-reports/{warehouseId}")]
        [Authorize(Roles = "WarehouseAdmin,SuperAdmin,StoreOwner")]
        public async Task<IActionResult> GetReports([FromRoute] int warehouseId)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository.GetUserByAccountId(userAppId);
            var reports = await _recieveProductReport.GetReciveProductReports(warehouseId, hardwareStoreUserInfo.HardwareStoreId);
            return Ok(ConvertToJson(reports));
        } 
        [HttpGet]
        [Route("get-report/{warehouseReportId}")]
        [Authorize(Roles = "WarehouseAdmin,SuperAdmin,StoreOwner")]
        public async Task<IActionResult> GetReport([FromRoute] int warehouseReportId)
        {
            var report = await _recieveProductReport.GetReciveProductReport(warehouseReportId);
            return Ok(ConvertToJson(report));
        } 
        [HttpGet]
        [Route("get-recieveproducts/{warehouseReportId}")]
        [Authorize(Roles = "WarehouseAdmin,SuperAdmin,StoreOwner")]
        public async Task<IActionResult> GetRecieveProducts([FromRoute] int warehouseReportId)
        {
            var recieveProducts = await _recieveProductReport.GetRecieveProducts(warehouseReportId);
            return Ok(ConvertToJson(recieveProducts));
        }
        [HttpGet]
        [Route("get-recieveproduct/{recieveProductId}")]
        [Authorize(Roles = "WarehouseAdmin,SuperAdmin,StoreOwner")]
        public async Task<IActionResult> GetRecieveProduct([FromRoute] int recieveProductId)
        {
            var recieveProduct = await _recieveProductReport.GetRecieveProduct(recieveProductId);
            return Ok(ConvertToJson(recieveProduct));
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
