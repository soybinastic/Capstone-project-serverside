using ConstructionMaterialOrderingApi.Dtos.HardwareProductDtos;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IHardwareProductRepository
    {
        Task Add(HardwareProductDto productDto);
        Task<bool> Delete(int hardwareStoreId, int hardwareProductId, int warehouseId);
        Task<HardwareProduct> GetProduct(int hardwareProductId);
        Task<List<HardwareProduct>> GetProducts(int hardwareStoreId);
        Task<bool> Update(HardwareProductDto productDto, int hardwareProductId, int hardwareStoreId);
        Task<string> UploadProductImage(int hardwareProductId, string dbPath,int hardwareStoreId);
    }
}