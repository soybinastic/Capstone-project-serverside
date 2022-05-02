using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class WareHouseAdminRepository : IWareHouseAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public WareHouseAdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddWareHouseAdmin(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId)
        {
            var newWareHouseAdmin = new WarehouseAdmin()
            {
                AccountId = applicationUserId,
                HardwareStoreId = hardwareStoreId,
                WarehouseId = storeUserDto.WarehouseId,
                Name = storeUserDto.FirstName + " " + storeUserDto.LastName
            };
            _context.WarehouseAdmins.Add(newWareHouseAdmin);
            await _context.SaveChangesAsync();
        }
        public async Task<WarehouseAdmin> GetWarehouseAdminByAccountId(string accountId)
        {
            var warehouseAdmin = await _context.WarehouseAdmins.Where(a => a.AccountId == accountId)
                .FirstOrDefaultAsync();
            if (warehouseAdmin != null)
            {
                return warehouseAdmin;
            }
            else
            {

                throw new Exception("Not Found.");
            }
        }
        public async Task<WarehouseAdmin> GetWareHouseAdminByHardwareStoreId(int storeId)
        {
            var warehouseAdmin = await _context.WarehouseAdmins.Where(a => a.HardwareStoreId == storeId)
                .FirstOrDefaultAsync();
            if (warehouseAdmin != null)
            {
                return warehouseAdmin;
            }
            else
            {
                throw new Exception("Not Found");
            }
        }
    }
}
