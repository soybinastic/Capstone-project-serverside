using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.DeliverProductDtos;
using Microsoft.EntityFrameworkCore;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class DeliverProductReportRepository : IDeliverProductReportRepository
    {
        private readonly ApplicationDbContext _context;
        public DeliverProductReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(DeliverProductDto deliverProductDto,
            int warehouseId, int hardwareStoreId, int warehouseProductId)
        {
            var dateNow = DateTime.Now;
            var deliverProductReport = await _context.WarehouseReports.Where(dpr => dpr.WarehouseId == warehouseId
                && dpr.DateReported.Year == dateNow.Year
                && dpr.DateReported.Day == dateNow.Day
                && dpr.DateReported.Month == dateNow.Month
                && dpr.ReportType == "DeliverProductReport")
                .FirstOrDefaultAsync();

            if (deliverProductReport != null)
            {
                var deliverProduct = new DeliverProduct()
                {
                    WarehouseReportId = deliverProductReport.Id,
                    BranchId = deliverProductDto.BranchId,
                    WarehouseId = warehouseId,
                    WarehouseProductId = warehouseProductId,
                    Quantity = deliverProductDto.Quantity
                };

                _context.DeliverProducts.Add(deliverProduct);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newDeliverProductReport = new WarehouseReport()
                {
                    Description = "None",
                    HardwareStoreId = hardwareStoreId,
                    WarehouseId = warehouseId,
                    ReportType = "DeliverProductReport",
                    DateReported = DateTime.Now
                };

                _context.WarehouseReports.Add(newDeliverProductReport);
                await _context.SaveChangesAsync();

                var deliverProduct = new DeliverProduct()
                {
                    WarehouseReportId = newDeliverProductReport.Id,
                    BranchId = deliverProductDto.BranchId,
                    WarehouseId = warehouseId,
                    WarehouseProductId = warehouseProductId,
                    Quantity = deliverProductDto.Quantity
                };

                _context.DeliverProducts.Add(deliverProduct);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<List<DeliverProductReportDto>> GetDeliverProducts(int warehouseId)
        {
            var reports = new List<DeliverProductReportDto>();
            var deliverProductReports = await _context.WarehouseReports
                .Where(dpr => dpr.WarehouseId == warehouseId && dpr.ReportType == "DeliverProductReport")
                .OrderByDescending(dpr => dpr.DateReported)
                .ToListAsync();

            foreach (var deliverProductReport in deliverProductReports)
            {
                reports.Add(new DeliverProductReportDto()
                {
                    ReportDetails = deliverProductReport,
                    DeliverProducts = _context.DeliverProducts.Where(dp => dp.WarehouseReportId == deliverProductReport.Id)
                    .Include(dp => dp.Branch)
                    .Include(dp => dp.WarehouseProduct)
                    .ThenInclude(dp => dp.HardwareProduct)
                    .ToList()
                });
            }

            return reports;

        }
    }
}
