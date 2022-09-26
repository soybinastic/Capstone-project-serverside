using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.BranchDto;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.AspNetCore.Http;
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
                DateRegistered = DateTime.Now,
                Image = branchDto.Image != null ? await UploadFile(branchDto.Image, "BranchImages") : ""
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
            Console.WriteLine("File length: " + branchDto.Image?.Length);
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
                    branchToUpdate.Image = branchDto.Image != null ? await UploadFile(branchDto.Image, "BranchImages") : branchToUpdate.Image;
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

        private async Task<string> UploadFile(IFormFile file, string folderNamePath)
        {
            try
            {
                var folderName = Path.Combine("Resources", folderNamePath);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return dbPath;
                }

                return "";

            }
            catch (Exception e)
            {
                return "";
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
