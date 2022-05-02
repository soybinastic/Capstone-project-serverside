using ConstructionMaterialOrderingApi.Dtos.WarehouseDto;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class WarehouseController : ControllerBase
    {
        private readonly IHardwareStoreUserRepository _hardwareStoreUserRepository;
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseController(IHardwareStoreUserRepository hardwareStoreUserRepository,
            IWarehouseRepository warehouseRepository)
        {
            _hardwareStoreUserRepository = hardwareStoreUserRepository;
            _warehouseRepository = warehouseRepository;
        } 

        [HttpGet]
        [Route("get-warehouses")]
        [Authorize(Roles = "WarehouseAdmin,StoreOwner,SuperAdmin,StoreAdmin")]
        public async Task<IActionResult> GetWarehouses()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository.GetUserByAccountId(userId);
            var warehouses = await _warehouseRepository.GetWarehouses(hardwareStoreUserInfo.HardwareStoreId);
            var jsonResult = ConvertToJson(warehouses);
            return Ok(jsonResult);
        }
        [HttpPost]
        [Route("add-warehouse")]
        [Authorize(Roles = "StoreOwner,SuperAdmin")]
        public async Task<IActionResult> AddWarehouse([FromBody] WarehouseDto warehouseDto)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository.GetUserByAccountId(userId);
            await _warehouseRepository.AddWarehouse(warehouseDto, hardwareStoreUserInfo.HardwareStoreId);
            return Ok(new { Success = 1, Message = "Added Successfully."});
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
