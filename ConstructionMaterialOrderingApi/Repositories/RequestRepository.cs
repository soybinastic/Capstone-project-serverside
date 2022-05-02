using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.RequestDtos;
using Microsoft.EntityFrameworkCore;
using ConstructionMaterialOrderingApi.Models;
using System.Text;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IRequestProductRepository _requestProductRepository;
        private readonly INotificationRepository _notificationRepository;

        public RequestRepository(ApplicationDbContext context,
            IRequestProductRepository requestProductRepository,
            INotificationRepository notificationRepository)
        {
            _context = context;
            _requestProductRepository = requestProductRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<(bool, string)> Send(RequestProductDto requestProductDto, int branchId)
        {
            var dateNow = DateTime.Now;

            var isProductExist = await _context.WarehouseProducts.AnyAsync(wp => wp.WarehouseId == requestProductDto.WarehouseId
                && wp.HardwareProductId == requestProductDto.HardwareProductId);
            if (isProductExist)
            {
                var isActive = await _context.WarehouseProducts.AnyAsync(wp => wp.WarehouseId == requestProductDto.WarehouseId
                && wp.HardwareProductId == requestProductDto.HardwareProductId
                && wp.IsActive);
                if (isActive)
                {
                    string message = "";
                    var todayRequest = await _context.Requests.Where(r => r.WarehouseId == requestProductDto.WarehouseId
                        && r.BranchId == branchId
                        && r.DateRequest.Year == dateNow.Year
                        && r.DateRequest.Month == dateNow.Month
                        && r.DateRequest.Day == dateNow.Day)
                        .FirstOrDefaultAsync();

                    var warehouseProduct = await _context.WarehouseProducts.FirstOrDefaultAsync(wp => wp.WarehouseId == requestProductDto.WarehouseId
                        && wp.HardwareProductId == requestProductDto.HardwareProductId);

                    if (warehouseProduct.StockNumber > 0)
                    {
                        message = "Your request has been sent successully.";
                    }
                    else
                    {
                        message = "Your request has been sent successully. Note: This product is out of stock yet in this warehouse";
                    }


                    if (todayRequest != null)
                    {
                        await _requestProductRepository.Add(todayRequest.Id, requestProductDto);
                    }
                    else
                    {
                        var newRequest = new Request()
                        {
                            BranchId = branchId,
                            WarehouseId = requestProductDto.WarehouseId,
                            DateRequest = DateTime.Now
                        };

                        _context.Requests.Add(newRequest);
                        await _context.SaveChangesAsync();

                        await _requestProductRepository.Add(newRequest.Id, requestProductDto);
                    }

                    var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId);

                    StringBuilder notificationText = new StringBuilder();
                    notificationText.Append(branch.Name);
                    notificationText.Append(" sent product request. Please check Reports -> Branch Requests");

                    await _notificationRepository.PushWarehouseNotification(requestProductDto.WarehouseId, notificationText.ToString(), "Request");

                    return (true, message);
                }
                else
                {
                    return (false, "The product you are requesting is not active yet.");
                }
            }

            return (false, "The product you are requesting this warehouse was not available.");

        }

        public async Task<List<Request>> GetAllRequestsByBranchId(int branchId)
        {
            var requests = await _context.Requests.Where(r => r.BranchId == branchId)
                .Include(r => r.Branch)
                .Include(r => r.Warehouse)
                .Include(r => r.RequestProducts)
                .ThenInclude(rp => rp.HardwareProduct)
                .OrderByDescending(r => r.DateRequest)
                .ToListAsync();

            return requests;
        }

        public async Task<List<Request>> GetAllRequestsByWarehouseId(int warehouseId)
        {
            var requests = await _context.Requests.Where(r => r.WarehouseId == warehouseId)
                .Include(r => r.Branch)
                .Include(r => r.Warehouse)
                .Include(r => r.RequestProducts)
                .ThenInclude(rp => rp.HardwareProduct)
                .OrderByDescending(r => r.DateRequest)
                .ToListAsync();

            return requests;
        }
    }
}
