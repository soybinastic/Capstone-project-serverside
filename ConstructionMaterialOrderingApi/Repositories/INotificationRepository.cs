using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface INotificationRepository
    {
        Task CreateNotificationNumber(int branchId);
        Task<NotificationNumber> GetNotificationNumber(int branchId);
        Task<List<BranchNotification>> GetNotifications(int branchId);
        Task PushNotification(int branchId, string text, string type);
        Task CreateWarehouseNotificationNumber(int warehouseId);
        Task PushWarehouseNotification(int warehouseId, string text, string type);
        Task<List<WarehouseNotification>> GetWarehouseNotifications(int warehouseId);
        Task<WarehouseNotificationNumber> GetWarehouseNotificationNumber(int warehouseId);
    }
}