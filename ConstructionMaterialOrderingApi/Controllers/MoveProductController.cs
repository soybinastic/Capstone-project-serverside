using ConstructionMaterialOrderingApi.Dtos.MoveProductDto;
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
    public class MoveProductController : ControllerBase
    {
        private readonly IMoveProductRepository _moveProductRepository;
        private readonly IHardwareStoreUserRepository _hardwareStoreUserRepository;

        public MoveProductController(IMoveProductRepository moveProductRepository, 
            IHardwareStoreUserRepository hardwareStoreUserRepository)
        {
            _moveProductRepository = moveProductRepository;
            _hardwareStoreUserRepository = hardwareStoreUserRepository;
        } 

        [HttpPost]
        [Route("move-product")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> MoveProduct([FromBody] MoveProductDto moveProductDto)
        {
            var appUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdminInfo = await _hardwareStoreUserRepository
                .GetUserByAccountId(appUserId);

            var result = await _moveProductRepository.MoveProductToWarehouse(moveProductDto, warehouseAdminInfo.HardwareStoreId);
            return result ? Ok(new {Success = 1, Message = "Moved Successfully"}) : BadRequest(new { Success = 0, Message = "Something went wrong."});
        } 

        [HttpGet]
        [Route("get-moveproducts/{warehouseId}")]
        [Authorize(Roles = "WarehouseAdmin,SuperAdmin,StoreOwner")]
        public async Task<IActionResult> GetMoveProducts([FromRoute] int warehouseId)
        {
            var moveProducts = await _moveProductRepository.GetMoveProducts(warehouseId);
            return Ok(ConvertToJson(moveProducts));
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
