using ConstructionMaterialOrderingApi.Dtos.HardwareProductDtos;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStorageController : ControllerBase
    {
        private readonly IHardwareProductRepository _hardwareProductRepository;
        private readonly IHardwareStoreUserRepository _hardwareStoreUserRepository;
        private readonly IWarehouseProductRepository _warehouseProductRepository;

        public ProductStorageController(IHardwareProductRepository hardwareProductRepository,
            IHardwareStoreUserRepository hardwareStoreUserRepository,
            IWarehouseProductRepository warehouseProductRepository)
        {
            _hardwareProductRepository = hardwareProductRepository;
            _hardwareStoreUserRepository = hardwareStoreUserRepository;
            _warehouseProductRepository = warehouseProductRepository;
        } 

        [HttpPost]
        [Route("add-product")]
        [Authorize(Roles = "StoreOwner,SuperAdmin,WarehouseAdmin")]
        public async Task<IActionResult> AddProduct([FromBody] HardwareProductDto productDto)
        {
            var appUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository
                .GetUserByAccountId(appUserId);

            productDto.HardwareStoreId = hardwareStoreUserInfo.HardwareStoreId;

            await _hardwareProductRepository.Add(productDto);

            return Ok(new { Success = 1, Message = "Added successfully"});
        } 

        [HttpGet]
        [Route("get-products/{warehouseId}")]
        [Authorize(Roles = "StoreOwner,SuperAdmin,WarehouseAdmin")]
        public async Task<IActionResult> GetProducts(int warehouseId)
        {
            var products = await _warehouseProductRepository
                .GetProducts(warehouseId);
            var jsonReuslt = ConvertToJson(products);
            return Ok(jsonReuslt);
        } 
        [HttpGet]
        [Route("get-product/{warehouseId}/{productId}")]
        [Authorize(Roles = "StoreOwner,SuperAdmin,WarehouseAdmin")]
        public async Task<IActionResult> GetProduct(int warehouseId, 
            int productId)
        {
            var product = await _warehouseProductRepository
                .GetProduct(warehouseId, productId);
            var jsonResult = ConvertToJson(product);
            return Ok(jsonResult);
        } 
        [HttpPut]
        [Route("update-product/{productId}")]
        [Authorize(Roles = "StoreOwner,SuperAdmin,WarehouseAdmin")]
        public async Task<IActionResult> UpdateProduct([FromBody] HardwareProductDto productDto, 
            int productId)
        {

            var appUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository
                .GetUserByAccountId(appUserId);
            var result = await _hardwareProductRepository
                .Update(productDto, productId, hardwareStoreUserInfo.HardwareStoreId);

            return result ? Ok(new { Success = 1, Message = "Updated successfully" }) :
                BadRequest(new { Success = 0, Message = "Failed to update"});
        }
        
        [HttpPost]
        [Route("update-product-image/{hardwareProductId}")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> UpdateImage([FromRoute]int hardwareProductId)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var storeAdmin = await _hardwareStoreUserRepository.GetUserByAccountId(userAppId);
            try
            {
                string imageUrl = "";
                var file = Request.Form.Files[0];

                var folderName = Path.Combine("Resources","ProductImages");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if(!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
       
                if(file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString().Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        
                    }

                    
                    imageUrl = await _hardwareProductRepository.UploadProductImage(hardwareProductId, dbPath, storeAdmin.HardwareStoreId);


                }
                return Ok(ConvertToJson(imageUrl));

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete-product/{warehouseId}/{productId}")]
        [Authorize(Roles = "StoreOwner,SuperAdmin,WarehouseAdmin")]
        public async Task<IActionResult> DeleteProduct(int warehouseId, int productId)
        {
            var appUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUserInfo = await _hardwareStoreUserRepository
                .GetUserByAccountId(appUserId);
            var result = await _hardwareProductRepository.Delete(hardwareStoreUserInfo.HardwareStoreId,
                productId, warehouseId);

            return result ? Ok(new { Success = 1, Message = "Deleted Successfully" }) :
                BadRequest(new { Success = 0, Message = "Failed to delete."});
        } 

        [HttpGet]
        [Route("available-products/{warehouseId}")]
        [Authorize(Roles = "SuperAdmin,StoreOwner,WarehouseAdmin")]
        public async Task<IActionResult> GetAvailableProducts([FromRoute]int warehouseId)
        {
            var availableProducts = await _warehouseProductRepository
                .GetAvailableProducts(warehouseId);
            return Ok(ConvertToJson(availableProducts));
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
