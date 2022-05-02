using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.BranchDto;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationRepository _notificationRepository;

        public BranchRepository(ApplicationDbContext context, INotificationRepository notificationRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
        }

        public async Task AddBranch(AddBranchDto branchDto, int hardwareStoreId)
        {
            var newBranch = new Branch()
            {
                HardwareStoreId = hardwareStoreId,
                Name = branchDto.BranchName,
                Address = branchDto.Address,
                IsActive = branchDto.IsActive
            };
            _context.Branches.Add(newBranch);
            await _context.SaveChangesAsync();

            await _notificationRepository.CreateNotificationNumber(newBranch.Id);

            var orderNotificationNumber = new OrderNotificationNumber()
            {
                BranchId = newBranch.Id,
                HardwareStoreId = hardwareStoreId,
                NumberOfOrder = 0,
            };

            _context.OrderNotificationNumbers.Add(orderNotificationNumber);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Branch>> GetBranchesByStoreId(int hardwareStoreId)
        {
            var branches = await _context.Branches.Where(b => b.HardwareStoreId == hardwareStoreId)
                .ToListAsync();
            return branches;
        }

        public async Task<Branch> GetBranch(int branchId)
        {
            var branch = await _context.Branches.Where(b => b.Id == branchId)
                .FirstOrDefaultAsync();
            return branch;
        }

        public async Task<List<Branch>> GetActiveBranches(int hardwareStoreId)
        {
            var activeBranches = await _context.Branches
                .Where(b => b.HardwareStoreId == hardwareStoreId && b.IsActive)
                .ToListAsync();
            return activeBranches;
        }

        public async Task<bool> UpdateBranch(UpdateBranchDto branchDto, int branchId, int hardwareStoreId)
        {
            if(branchDto.Id == branchId && branchId != 0 && branchDto.Id != 0)
            {
                var branchToUpdate = await _context.Branches
                    .Where(b => b.Id == branchId && b.HardwareStoreId == hardwareStoreId)
                    .FirstOrDefaultAsync();
                if(branchToUpdate != null)
                {
                    branchToUpdate.Name = branchDto.BranchName;
                    branchToUpdate.Address = branchDto.Address;
                    branchToUpdate.IsActive = branchDto.IsActive;
                    await _context.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
