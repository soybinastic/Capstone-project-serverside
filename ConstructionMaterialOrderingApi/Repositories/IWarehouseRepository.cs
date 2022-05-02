using ConstructionMaterialOrderingApi.Dtos.WarehouseDto;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IWarehouseRepository
    {
        Task AddWarehouse(WarehouseDto warehouseDto, int hardwareStoreId);
        Task<Warehouse> GetWarehouse(int warehouseId, int hardwareStoreId);
        Task<List<Warehouse>> GetWarehouses(int hardwareStoreId);
    }
}