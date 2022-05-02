using ConstructionMaterialOrderingApi.Dtos.HardwareProductDtos;
using ConstructionMaterialOrderingApi.Dtos.MoveProductDto;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IWarehouseProductRepository
    {
        Task Add(HardwareProductDto productDto, int hardwareProductId);
        Task<WarehouseProduct> GetProduct(int warehouseId, int hardwareProductId);
        Task<List<WarehouseProduct>> GetProducts(int warehouseId);
        Task<bool> MoveProductToWarehouse(MoveProductDto moveProductDto, int hardwareStoreId);
        Task Update(HardwareProductDto productDto, int hardwareProductId);
        Task Delete(int hardwareProductId, int warehouseId);
        Task<List<WarehouseProduct>> GetAvailableProducts(int warehouseId);
        Task<List<WarehouseProduct>> GetProductsByHardwareProductId(int hardwareProductId);
    }
}