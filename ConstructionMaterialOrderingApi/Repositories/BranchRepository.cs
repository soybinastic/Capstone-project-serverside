using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.BranchDto;
using ConstructionMaterialOrderingApi.Helpers;
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
                IsActive = branchDto.IsActive,
                DateRegistered = DateTime.Now
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
                    branchToUpdate.Latitude = branchDto.Lat;
                    branchToUpdate.Longitude = branchDto.Lng;
                    branchToUpdate.Range = branchDto.Range;
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

        public async Task<List<BranchDto>> GetAllBranches(double lat = 0, double lng = 0, double adjustedKm = 0)
        {
            var branches = await _context.Branches.ToListAsync();

            if(lat == 0 || lng == 0)
            {
                var branchesDto = branches.Select(b => new BranchDto(b.Id, b.HardwareStoreId, b.Name, b.Address, b.IsActive, b.DateRegistered,
                    b.Longitude, b.Latitude, b.Range, b.Image, 0))
                    .ToList();

                return branchesDto;
            }

            // nearBranches = nearBranches
            //     .Where(b => Coordinates.DistanceBetweenPlaces(lng, lat, b.Longitude, b.Latitude) <= 5)
            //     .ToList();
            var nearBranches = branches
                .Where(b => Coordinates.DistanceBetweenPlaces(lng, lat, b.Longitude, b.Latitude) <= adjustedKm
                && b.Range >= Coordinates.DistanceBetweenPlaces(lng, lat, b.Longitude, b.Latitude)
                && b.IsActive)
                .Select(b => new BranchDto(b.Id, b.HardwareStoreId, b.Name, b.Address, b.IsActive, b.DateRegistered,
                    b.Longitude, b.Latitude, b.Range, b.Image, Coordinates.DistanceBetweenPlaces(lng, lat, b.Longitude, b.Latitude)))
                .ToList();

            return nearBranches;
        }
        public async Task<List<BranchDto>> Search(string search, double lat = 0, double lng = 0, double adjustedKm = 0)
        {
            var branches = await GetAllBranches(lat, lng, adjustedKm);
            //search = search.Trim('\'');
            if(string.IsNullOrWhiteSpace(search))
            {
                return branches;
            }

            branches = branches.Where(b => b.Name.ToLower().Contains(search.ToLower()) || b.Address.ToLower().Contains(search.ToLower())).ToList();
            return branches;
        }
    }
}
