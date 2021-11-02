using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.CategoryDtos;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateCategory(int hardwareStoreId, CreateCategoryDto model)
        {
            TextInfo text = new CultureInfo("en-US", false).TextInfo;
            model.CategoryName = text.ToTitleCase(model.CategoryName);
            var categoryIsExist = await _context.Categories.Where(c => c.HardwareStoreId == hardwareStoreId && c.Name == model.CategoryName)
                .FirstOrDefaultAsync(); 
            if(categoryIsExist != null)
            {
                return false;
            }

            var category = new Category()
            {
                HardwareStoreId = hardwareStoreId,
                Name = model.CategoryName
            };

            await _context.Categories.AddAsync(category);
            _context.SaveChanges();
            return true;
        }

        public async Task<List<CategoryDto>> GetCategories(int hardwareStoreId)
        {
            var listOfCategories = new List<CategoryDto>();
            var categories = await _context.Categories.Where(c => c.HardwareStoreId == hardwareStoreId)
                .ToListAsync();
            categories.ForEach((category) =>
            {
                var categoryDto = new CategoryDto()
                {
                    CategoryId = category.Id,
                    HardwareStoreId = category.HardwareStoreId,
                    CategoryName = category.Name
                };

                listOfCategories.Add(categoryDto);
            });
            return listOfCategories;
        }

        public async Task<CategoryDto> GetCategory(int hardwareStoreId, int categoryId)
        {
            var category = await _context.Categories.Where(c => c.Id == categoryId && c.HardwareStoreId == hardwareStoreId)
                .FirstOrDefaultAsync();

            var categoryDto = new CategoryDto()
            {
                CategoryId = category.Id,
                CategoryName = category.Name
            };

            return categoryDto;
        }
    }
}
