using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class StoreAdminRepository : IStoreAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreAdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddStoreAdmin(HardwareStoreUserDto storeUserDto, string applicationUserId,
            int hardwareStoreUserId)
        {
            var newStoreAdmin = new StoreAdmin()
            {
                AccountId = applicationUserId,
                HardwareStoreId = hardwareStoreUserId,
                BranchId = storeUserDto.BranchId,
                Name = storeUserDto.FirstName + " " + storeUserDto.LastName
            };

            _context.StoreAdmins.Add(newStoreAdmin);
            await _context.SaveChangesAsync();
        }
        public async Task<StoreAdmin> GetStoreAdminByAccountId(string applicationUserId)
        {
            var storeAdmin = await _context.StoreAdmins
                .Where(sa => sa.AccountId == applicationUserId)
                .FirstOrDefaultAsync();
            if (storeAdmin != null)
            {
                return storeAdmin;
            }
            else
            {
                throw new Exception("Not found");
            }
        }
    }
}
