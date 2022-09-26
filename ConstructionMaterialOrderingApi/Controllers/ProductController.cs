using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using ConstructionMaterialOrderingApi.Dtos.ProductDtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IHardwareStoreRepository _hardwareStoreRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(IProductRepository productRepository, IHardwareStoreRepository hardwareStoreRepository,
            UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _hardwareStoreRepository = hardwareStoreRepository;
            _userManager = userManager;
        }
        [HttpGet]
        [Route("get-hardwareproducts/{branchId}")]
        public async Task<IActionResult> GetHardwareProducts([FromRoute] int branchId, [FromQuery(Name = "search")]string search)
        {
            var products = await _productRepository.GetHardwareProducts(branchId);
            if(string.IsNullOrWhiteSpace(search))
            {
                return Ok(ConvertToJson(products));
            }
            products = products.Where(p => p.Name.ToLower().Contains(search.ToLower())).ToList();
            return Ok(ConvertToJson(products));
        }
        [HttpGet]
        [Route("get-hardwareproduct/{branchId}/{productId}")]
        public async Task<IActionResult> GetHardwareProduct([FromRoute] int branchId, [FromRoute] int productId)
        {
            var product = await _productRepository.GetHardwareProduct(branchId, productId);
            return Ok(ConvertToJson(product));
        }
        [HttpGet]
        [Route("get-hardwareproduct-by-category/{branchId}/{categoryId}")]
        public async Task<IActionResult> GetHardwareProductByCategory([FromRoute]int branchId, [FromRoute]int categoryId)
        {
            // var queryString = HttpContext.Request.Query["search"].FirstOrDefault();
            var products = await _productRepository.GetHardwareProductByCategory(branchId, categoryId);
            
            // if(!string.IsNullOrWhiteSpace(queryString))
            // {
            //     products = products.Where(p => p.Name.Contains(queryString)).ToList();
            // }

            return Ok(ConvertToJson(products));
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

        [HttpGet]
        [Route("/api/product/get-product/{storeId}/{productId}")]
        public async Task<IActionResult> GetProduct(int storeId, int productId)
        {
            var product = await _productRepository.GetProduct(storeId, productId);

            var productJsonObject = JsonConvert.SerializeObject(product, new JsonSerializerSettings 
            { 
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return Ok(productJsonObject);
        } 

        [HttpGet]
        [Route("/api/product/get-products/{storeId}")]
        public async Task<IActionResult> GetAllProduct(int storeId)
        {
            var products = await _productRepository.GetAllProduct(storeId);
            var productsJsonObject = JsonConvert.SerializeObject(products, new JsonSerializerSettings 
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return Ok(productsJsonObject);
        } 

        [HttpGet]
        [Route("/api/product/get-products-by-category/{storeId}/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int storeId, int categoryId)
        {
            var products = await _productRepository.GetAllProductByCategory(storeId, categoryId);
            var productsJsonObject = JsonConvert.SerializeObject(products, new JsonSerializerSettings 
            { 
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return Ok(productsJsonObject);
        }

        [HttpPost]
        [Route("/api/product/add-product")]
        [Authorize(Roles = "StoreOwner")]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductDto model)
        {
            var hardwareStoreAdminId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userAdmin = await _userManager.FindByIdAsync(hardwareStoreAdminId);
            var role = await _userManager.GetRolesAsync(userAdmin); 
            if(role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _hardwareStoreRepository.GetHardware(userAdmin.Id);
                var result = await _productRepository.CreateProduct(hardwareStore.HardwareStoreId, model);
                return result ? Ok(new { Success = 1, Message = "Product added successfully" }) : BadRequest(new { Success = 0, Message = "Failed to add a product" });
            } 

            return BadRequest(new { Success = 0, Message = "Something went wrong." });
        }

        [HttpPut]
        [Route("/api/product/update-product/{productId}")]
        [Authorize(Roles = "StoreOwner")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] UpdateProductDto model)
        {
            var hardwareStoreAdminId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreAdminId);

            var role = await _userManager.GetRolesAsync(hardwareStoreUser); 
            if(role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _hardwareStoreRepository.GetHardware(hardwareStoreUser.Id);
                var result = await _productRepository.UpdateProduct(hardwareStore.HardwareStoreId, productId, model);

                return result ? Ok(new { Success = 1, Message = "Product has been successfully updated." }) : BadRequest(new { Success = 0, Message = "Failed to update product." });
            }
            return BadRequest(new { Success = 0, Message = "Something went wrong"});
        }

        [HttpDelete]
        [Route("/api/product/delete-product/{productId}")]
        [Authorize(Roles = "StoreOwner")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var hardwareStoreAdminId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreAdminId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser); 

            if(role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _hardwareStoreRepository.GetHardware(hardwareStoreUser.Id);
                var result = await _productRepository.DeleteProduct(hardwareStore.HardwareStoreId, productId);

                return result ? Ok(new { Success = 1, Message = "Products deleted successfully."}) : BadRequest(new { Success = 0, Message = "Failed to delete product."});
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong."});
        }
    }
}
