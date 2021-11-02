using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.AdminDtos;
using ConstructionMaterialOrderingApi.Dtos.HardwareStoreDtos;
using ConstructionMaterialOrderingApi.Hubs;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class HardwareStoreRepository : IHardwareStoreRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<HardwareStoreHub> _hubContext;

        public HardwareStoreRepository(ApplicationDbContext context,IHubContext<HardwareStoreHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public void AddHardwareStore(string accountId, RegisterHardwareStoreDto model)
        {
            var hardwareStore = new HardwareStore()
            {
                AccountId = accountId,
                Email = model.Email,
                HardwareStoreName = model.HardwareStoreName,
                Owner = model.Owner,
                BusinessAddress = model.BusinessAddress,
                ContactNo = model.ContactNo,
                IsOpen = false
            };
            _context.HardwareStores.Add(hardwareStore);
            _context.SaveChanges();

            //find hardware store after added to database to get hardwareId to store it to order notification entity.
            var hardware = _context.HardwareStores.Where(h => h.AccountId == accountId).FirstOrDefault();

            var orderNotification = new OrderNotificationNumber()
            {
                HardwareStoreId = hardware.Id,
                NumberOfOrder = 0
            };
            _context.OrderNotificationNumbers.Add(orderNotification);
            _context.SaveChanges();
        }

        public async Task<List<GetAllHardwareStoreDto>> GetAllHardwareStore()
        {
            var hardwareStoreList = new List<GetAllHardwareStoreDto>();
            var hardwareStores = await _context.HardwareStores.ToListAsync();
            hardwareStores.ForEach((hardwareStore) =>
            {
                var hardware = new GetAllHardwareStoreDto()
                {
                    HardwareStoreId = hardwareStore.Id,
                    Owner = hardwareStore.Owner,
                    HardwareStoreName = hardwareStore.HardwareStoreName,
                    BusinessAddress = hardwareStore.BusinessAddress,
                    Email = hardwareStore.Email,
                    ContactNo = hardwareStore.ContactNo,
                    IsOpen = hardwareStore.IsOpen
                };
                hardwareStoreList.Add(hardware);
            });
            return hardwareStoreList;
        }

        public async Task<GetHardwareStoreDto> GetHardware(string accountId)
        {
            var hardwareStore = await _context.HardwareStores.Where(hardware => hardware.AccountId == accountId)
                .FirstOrDefaultAsync(); 
            if(hardwareStore != null)
            {
                var hardwareDto = new GetHardwareStoreDto()
                {
                    HardwareStoreId = hardwareStore.Id,
                    AccountId = hardwareStore.AccountId,
                    Email = hardwareStore.Email,
                    BusinessAddress = hardwareStore.BusinessAddress,
                    ContactNo = hardwareStore.ContactNo,
                    Owner = hardwareStore.Owner,
                    HardwareStoreName = hardwareStore.HardwareStoreName,
                    IsOpen = hardwareStore.IsOpen
                };

                return hardwareDto;
            }
            return null;
        }

        public async Task<GetHardwareStoreDto> GetHardwareByStoreId(int hardwareStoreId)
        {
            var hardwareStore = await _context.HardwareStores.Where(hardware => hardware.Id == hardwareStoreId)
               .FirstOrDefaultAsync();
            if (hardwareStore != null)
            {
                var hardwareDto = new GetHardwareStoreDto()
                {
                    HardwareStoreId = hardwareStore.Id,
                    AccountId = hardwareStore.AccountId,
                    Email = hardwareStore.Email,
                    BusinessAddress = hardwareStore.BusinessAddress,
                    ContactNo = hardwareStore.ContactNo,
                    Owner = hardwareStore.Owner,
                    HardwareStoreName = hardwareStore.HardwareStoreName,
                    IsOpen = hardwareStore.IsOpen
                };

                return hardwareDto;
            }
            return null;
        }

        public async Task SignIn(string accountId, IList<string> role)
        {
            if(role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _context.HardwareStores.Where(h => h.AccountId == accountId)
                    .FirstOrDefaultAsync();
                hardwareStore.IsOpen = true;
                _context.SaveChanges();
                await _hubContext.Clients.All.SendAsync("RecieveStoreStatus", hardwareStore);
            }
        }

        public async Task SignOut(string accountId, IList<string> role)
        {
            if (role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _context.HardwareStores.Where(h => h.AccountId == accountId)
                    .FirstOrDefaultAsync();
                hardwareStore.IsOpen = false;
                _context.SaveChanges();
                await _hubContext.Clients.All.SendAsync("RecieveStoreStatus", hardwareStore);
            }
        }
    }
}
