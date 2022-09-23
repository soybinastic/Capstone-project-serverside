using System.Collections.Generic;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.OrderDtos;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IOrderPreparationRepository
    {
        Task<IEnumerable<AvailableSalesClerk>> GetAvailableSalesClerk(int branchId);
        Task<bool> SalesClerkAvailable(int salesClerkId);
        Task<List<OrderToPrepare>> GetAll(int salesClerkId);
        Task<OrderToPrepare> ToDeliver(int orderId);
    }
}