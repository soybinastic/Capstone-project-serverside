using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using ConstructionMaterialOrderingApi.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<HardwareStoreHub> _hubContext;
        public NotificationRepository(ApplicationDbContext context, IHubContext<HardwareStoreHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task PushNotification(int branchId, string text, string type)
        {
            var newNotifcation = new BranchNotification()
            {
                BranchId = branchId,
                Text = text,
                Type = type,
                DateNotified = DateTime.Now
            };

            _context.BranchNotifications.Add(newNotifcation);
            var notificationNumber = await _context.NotificationNumbers
                .Where(n => n.BranchId == branchId).FirstOrDefaultAsync();
            notificationNumber.Number += 1;
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("RecieveBranchNotifs", notificationNumber);
        }
        public async Task<List<BranchNotification>> GetNotifications(int branchId)
        {
            var notifs = await _context.BranchNotifications.Where(n => n.BranchId == branchId)
                .OrderByDescending(n => n.DateNotified)
                .ToListAsync();

            var notificationNumber = await _context.NotificationNumbers
                .Where(n => n.BranchId == branchId).FirstOrDefaultAsync();
            notificationNumber.Number = 0;
            await _context.SaveChangesAsync();

            return notifs;
        }
        public async Task<NotificationNumber> GetNotificationNumber(int branchId)
        {
            return await _context.NotificationNumbers.Where(n => n.BranchId == branchId)
                .FirstOrDefaultAsync();
        }
        public async Task CreateNotificationNumber(int branchId)
        {
            var newNotifNum = new NotificationNumber()
            {
                BranchId = branchId,
                Number = 0
            };
            _context.NotificationNumbers.Add(newNotifNum);
            await _context.SaveChangesAsync();
        }

        public async Task CreateWarehouseNotificationNumber(int warehouseId)
        {
            var newWarehouseNotifNum = new WarehouseNotificationNumber()
            {
                WarehouseId = warehouseId,
                Number = 0
            };
            _context.WarehouseNotificationNumbers.Add(newWarehouseNotifNum);
            await _context.SaveChangesAsync();
        }

        public async Task PushWarehouseNotification(int warehouseId, string text, string type)
        {
            var newNotification = new WarehouseNotification()
            {
                WarehouseId = warehouseId,
                Text = text,
                Type = type,
                DateNotified = DateTime.Now
            };

            _context.WarehouseNotifications.Add(newNotification);

            var notificationNumber = await _context.WarehouseNotificationNumbers
                .Where(n => n.WarehouseId == warehouseId).FirstOrDefaultAsync();

            notificationNumber.Number += 1;

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("RecieveWarehouseNotifs", notificationNumber);
        }

        public async Task<List<WarehouseNotification>> GetWarehouseNotifications(int warehouseId)
        {
            var notifs = await _context.WarehouseNotifications.Where(n => n.WarehouseId == warehouseId)
                .OrderByDescending(n => n.DateNotified)
                .ToListAsync();
            var notifNum = await _context.WarehouseNotificationNumbers.Where(n => n.WarehouseId == warehouseId)
                .FirstOrDefaultAsync();
            notifNum.Number = 0;
            await _context.SaveChangesAsync();

            return notifs;
        }

        public async Task<WarehouseNotificationNumber> GetWarehouseNotificationNumber(int warehouseId)
        {
            var notifNum = await _context.WarehouseNotificationNumbers.Where(n => n.WarehouseId == warehouseId)
                .FirstOrDefaultAsync();

            return notifNum;
        }
    }
}
