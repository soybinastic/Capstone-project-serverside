using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.CategoryDtos;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface ICategoryRepository
    {
        Task<bool> CreateCategory(int hardwareStoreId, CreateCategoryDto model);
        Task<List<CategoryDto>> GetCategories(int hardwareStoreId);
        Task<CategoryDto> GetCategory(int hardwareStoreId, int categoryId);
    }
}
