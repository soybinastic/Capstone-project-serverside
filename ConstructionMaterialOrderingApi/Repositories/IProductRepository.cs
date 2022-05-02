using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.ProductDtos;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IProductRepository
    {
        Task<bool> CreateHardwareProduct(int hardwareStoreId, HardwareProduct hardwareProduct, int quantity, int branchId);
        Task UpdateQuantity(int branchId, int hardwareProductId, int quantity);
        Task<bool> UpdateHardwareProduct(UpdateHardwareProductDto hardwareProductDto, int branchId, int hardwareProductId);
        Task<Product> GetHardwareProduct(int branchId, int hardwareProductId);
        Task<List<Product>> GetHardwareProducts(int branchId);
        Task<List<Product>> GetHardwareProductByCategory(int branchId, int categoryId);
        Task<bool> CreateProduct(int hardwareStoreId, CreateProductDto model);
        Task<bool> DeleteProduct(int hardwareStoreId, int productId);
        Task<bool> UpdateProduct(int hardwareStoreId, int productId, UpdateProductDto model);
        Task<GetProductDto> GetProduct(int hardwareStoreId, int productId);
        Task<List<GetProductDto>> GetAllProduct(int hardwareStoreId);
        Task<List<GetProductDto>> GetAllProductByCategory(int hardwareStoreId, int categoryId);
    }
}
