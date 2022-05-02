using ConstructionMaterialOrderingApi.Dtos.MoveProductDto;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IMoveProductRepository
    {
        Task<List<TransferProductDto>> GetMoveProducts(int warehouseId);
        Task<bool> MoveProductToWarehouse(MoveProductDto moveProductDto, int hardwareStoreId);
    }
}