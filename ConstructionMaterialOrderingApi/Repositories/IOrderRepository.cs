using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.OrderDtos;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> PostOrder(PostOrderDto model, int customerId);
        Task<List<GetOrderDto>> GetAllOrders(int hardwareStoreId);
        Task<List<GetCustomerOrderHistoryDto>> GetAllCustomerOrdersHistory(int customerId);
        Task<GetOrderNotificationNumber> GetOrderNotifNumber(int hardwareStoreId);
        Task<GetCustomerOrderDetails> GetCustomerOrderDetails(int hardwareStoreId, int orderId);
        Task<List<GetCustomerOrderProductDto>> GetCustomerOrderProducts(int hardwareStoreId, int orderId);
        Task<List<GetCustomerOrderProductHistoryDto>> GetCustomerOrderProductsHistory(int customerId, int orderId);
        Task<bool> UpdateOrder(int hardwareStoreId, int orderId, UpdateOrderDto model);
    }
}
