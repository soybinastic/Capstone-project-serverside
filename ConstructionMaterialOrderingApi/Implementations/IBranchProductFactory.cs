using ConstructionMaterialOrderingApi.Dtos.DeliverProductDtos;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public interface IBranchProductFactory
    {
        Task<bool> DeliverProduct(int warehouseId, int hardwareStoreId, DeliverProductDto deliverProductDto);
    }
}