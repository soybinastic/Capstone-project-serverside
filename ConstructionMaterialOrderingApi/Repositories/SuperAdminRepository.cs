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
    public class SuperAdminRepository : ISuperAdminRepository
    {
        private readonly ApplicationDbContext _context;
        public SuperAdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddSuperAdmin(HardwareStoreUserDto storeDto, string applicationUserId, int hardwareStoreId)
        {
            var newSuperAdmin = new SuperAdmin()
            {
                AccountId = applicationUserId,
                HardwareStoreId = hardwareStoreId,
                Name = storeDto.FirstName + " " + storeDto.LastName
            };

            _context.SuperAdmins.Add(newSuperAdmin);
            await _context.SaveChangesAsync();
        }
        public async Task<SuperAdmin> GetSuperAdminByAccountId(string accountId)
        {
            var superAdmin = await _context.SuperAdmins.Where(s => s.AccountId == accountId)
                .FirstOrDefaultAsync();
            if (superAdmin != null)
            {
                return superAdmin;
            }
            else
            {
                throw new Exception("Not Found");
            }
        }
        public async Task<SuperAdmin> GetSuperAdminByHardwareStoreId(int hardwareStoreId)
        {
            var superAdmin = await _context.SuperAdmins.Where(s => s.HardwareStoreId == hardwareStoreId)
               .FirstOrDefaultAsync();
            if (superAdmin != null)
            {
                return superAdmin;
            }
            else
            {
                throw new Exception("Not Found");
            }
        }
    }
}
