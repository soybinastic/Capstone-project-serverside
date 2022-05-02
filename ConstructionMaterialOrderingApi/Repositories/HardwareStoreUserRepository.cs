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
    public class HardwareStoreUserRepository : IHardwareStoreUserRepository
    {
        private readonly ApplicationDbContext _context;

        public HardwareStoreUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUser(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId)
        {
            var newHardwareStoreUser = new HardwareStoreUser()
            {
                ApplicationUserAccountId = applicationUserId,
                HardwareStoreId = hardwareStoreId,
                FirstName = storeUserDto.FirstName,
                LastName = storeUserDto.LastName,
                Email = storeUserDto.Email,
                Role = storeUserDto.Role,
                UserFrom = storeUserDto.UserFrom,
                UserName = storeUserDto.UserName
            };
            _context.HardwareStoreUsers.Add(newHardwareStoreUser);
            await _context.SaveChangesAsync();
        }
        public async Task<List<HardwareStoreUser>> GetUsers(int hardwareStoreId)
        {
            var users = await _context.HardwareStoreUsers.Where(u => u.HardwareStoreId == hardwareStoreId)
                .ToListAsync();
            return users;
        }
        public async Task<HardwareStoreUser> GetUserByAccountId(string applicationUserId)
        {
            var user = await _context.HardwareStoreUsers
                .Where(u => u.ApplicationUserAccountId == applicationUserId)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                return user;
            }
            else
            {
                throw new Exception($"UserId of {applicationUserId} was not found.");
            }

        }
        public async Task<HardwareStoreUser> GetUserByHardwareStoreId(int hardwareStoreId)
        {
            var user = await _context.HardwareStoreUsers.Where(u => u.HardwareStoreId == hardwareStoreId)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                return user;
            }
            else
            {
                throw new Exception($"User that having hardwarestoreId of {hardwareStoreId} was not found.");
            }
        }
        public async Task<HardwareStoreUser> GetUserById(int id)
        {
            var user = await _context.HardwareStoreUsers.Where(u => u.Id == id)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                return user;
            }
            else
            {
                throw new Exception($"User not found");
            }
        }

    }
}
