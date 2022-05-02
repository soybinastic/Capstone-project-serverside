using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.SaleDtos;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using ConstructionMaterialOrderingApi.Repositories;
using System.Text;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class BestSellingReportRepository : IBestSellingReportRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationRepository _notificationRepository;
        public BestSellingReportRepository(ApplicationDbContext context, INotificationRepository notificationRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
        }

        public async Task AddReport(BestSellingReportDto bestSellingReportDto, int branchId)
        {
            var newBestSellingReport = new BestSellingReport()
            {
                BranchId = branchId,
                WarehouseId = bestSellingReportDto.WarehouseId,
                DateReported = DateTime.Now,
                BestSellingReportType = bestSellingReportDto.BestSellingType
            };
            _context.BestSellingReports.Add(newBestSellingReport);
            await _context.SaveChangesAsync();

            foreach (var detail in bestSellingReportDto.BestSellingDetails)
            {
                var bestSellingDetail = new BestSellingDetail()
                {
                    BestSellingReportId = newBestSellingReport.Id,
                    BestSellingDate = detail.BestSellingDate
                };
                _context.BestSellingDetails.Add(bestSellingDetail);
                await _context.SaveChangesAsync();

                foreach (var bestSellingProduct in detail.BestSellingProducts)
                {
                    var bestSelling = new BestSellingProductReport()
                    {
                        BestSellingDetailId = bestSellingDetail.Id,
                        SaleId = bestSellingProduct.Id
                    };
                    _context.BestSellingProductReports.Add(bestSelling);
                    await _context.SaveChangesAsync();
                }
            }

            var branch = await _context.Branches.Where(b => b.Id == branchId).FirstOrDefaultAsync();
            StringBuilder notificationString = new StringBuilder();
            notificationString.Append(branch.Name);
            notificationString.Append(" send best selling products report.\nPlease check Reports-> Best Selling Products");

            await _notificationRepository.PushWarehouseNotification(bestSellingReportDto.WarehouseId, notificationString.ToString(), "BestSellingReport");

        }

        public async Task<List<BestSellingReport>> GetReportByBranchId(int branchId)
        {
            var reports = await _context.BestSellingReports.Where(b => b.BranchId == branchId)
                   .Include(b => b.Branch)
                   .Include(b => b.Warehouse)
                   .Include(b => b.BestSellingDetails)
                   .ThenInclude(b => b.BestSellingProductReports)
                   .ThenInclude(b => b.Sale)
                   .ThenInclude(b => b.HardwareProduct)
                   .OrderByDescending(b => b.DateReported)
                   .ToListAsync();
            return reports;
        }
        public async Task<List<BestSellingReport>> GetReportsByWarehouseId(int warehouseId)
        {
            var reports = await _context.BestSellingReports.Where(b => b.WarehouseId == warehouseId)
                   .Include(b => b.Branch)
                   .Include(b => b.Warehouse)
                   .Include(b => b.BestSellingDetails)
                   .ThenInclude(b => b.BestSellingProductReports)
                   .ThenInclude(b => b.Sale)
                   .ThenInclude(b => b.HardwareProduct)
                   .OrderByDescending(b => b.DateReported)
                   .ToListAsync();
            return reports;
        }
    }
}
