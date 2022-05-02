using ConstructionMaterialOrderingApi.Dtos.DeliverProductDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IDeliverProductReportRepository
    {
        Task Add(DeliverProductDto deliverProductDto, int warehouseId, int hardwareStoreId, int warehouseProductId);
        Task<List<DeliverProductReportDto>> GetDeliverProducts(int warehouseId);
    }
}