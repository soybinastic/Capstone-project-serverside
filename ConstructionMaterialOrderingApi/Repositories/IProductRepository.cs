using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.ProductDtos;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IProductRepository
    {
        Task<bool> CreateProduct(int hardwareStoreId, CreateProductDto model);
        Task<bool> DeleteProduct(int hardwareStoreId, int productId);
        Task<bool> UpdateProduct(int hardwareStoreId, int productId, UpdateProductDto model);
        Task<GetProductDto> GetProduct(int hardwareStoreId, int productId);
        Task<List<GetProductDto>> GetAllProduct(int hardwareStoreId);
        Task<List<GetProductDto>> GetAllProductByCategory(int hardwareStoreId, int categoryId);
    }
}
