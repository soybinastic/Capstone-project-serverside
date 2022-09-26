using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.OrderDtos;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> PostOrder(PostOrderDto model, int customerId);
        Task<Order> GetOrder(int orderId, int branchId);
        Task<Order> GetOrder(int orderId);
        Task<List<GetOrderDto>> GetAllOrders(int hardwareStoreId, int branchId);
        Task<List<GetCustomerOrderHistoryDto>> GetAllCustomerOrdersHistory(int customerId);
        Task<GetOrderNotificationNumber> GetOrderNotifNumber(int hardwareStoreId, int branchId);
        Task<GetCustomerOrderDetails> GetCustomerOrderDetails(int hardwareStoreId, int orderId, int branchId);
        Task<List<GetCustomerOrderProductDto>> GetCustomerOrderProducts(int hardwareStoreId, int orderId, int branchId);
        Task<List<GetCustomerOrderProductHistoryDto>> GetCustomerOrderProductsHistory(int customerId, int orderId);
        Task<(bool,string)> UpdateOrder(int hardwareStoreId, int orderId, UpdateOrderDto model, int hardwareStoreUserId);
        Task<bool> ApproveOrder(int branchId, int orderId, int salesClerkId);
    }
}
