using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;
using ConstructionMaterialOrderingApi.Dtos.CategoryDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHardwareStoreRepository _hardwareStoreRepository;

        public CategoryController(UserManager<ApplicationUser> userManager, ICategoryRepository categoryRepository,
            IHardwareStoreRepository hardwareStoreRepository)
        {
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _hardwareStoreRepository = hardwareStoreRepository;
        }

        [HttpPost]
        [Route("/api/category/create-category")]
        [Authorize(Roles = "StoreOwner")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto model)
        {
            var hardwareStoreUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser);
            if (role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareOwner = await _hardwareStoreRepository.GetHardware(hardwareStoreUser.Id);
                var result = await _categoryRepository.CreateCategory(hardwareOwner.HardwareStoreId, model);
                return result ? Ok(new { Success = 1, Message = "Category added successfully." }) : BadRequest(new { Success = 0, Message = "Failed to add category" });
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong" });
        }

        [HttpGet]
        [Route("/api/category/get-categories/{storeId}")]
        public async Task<IActionResult> GetCategories(int storeId)
        {
            var categories = await _categoryRepository.GetCategories(storeId);
            var categoriesJson = JsonConvert.SerializeObject(categories, new JsonSerializerSettings 
            { 
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return Ok(categoriesJson);
        }

        [HttpGet]
        [Route("/api/category/get-category/{storeId}/{categoryId}")]
        public async Task<IActionResult> GetCategory(int storeId, int categoryId)
        {
            var category = await _categoryRepository.GetCategory(storeId, categoryId);
            return Ok(category);
        }
    }
}
