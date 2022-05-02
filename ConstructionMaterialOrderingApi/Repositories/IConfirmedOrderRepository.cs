using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IConfirmedOrderRepository
    {
        Task Add(ConfirmedOrder confirmedOrder);
        Task<bool> ConfirmOrder(int orderId, int branchId);
        Task<List<ConfirmedOrder>> GetConfirmedOrders(int branchId);
        Task<ConfirmedOrder> GetConfirmedOrder(int branchId, int orderId);
    }
}