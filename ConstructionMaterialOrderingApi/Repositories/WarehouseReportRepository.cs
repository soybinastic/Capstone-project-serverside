using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Dtos.WarehouseReportDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class WarehouseReportRepository : IWarehouseReportRepository
    {
        private readonly ApplicationDbContext _context;

        public WarehouseReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WarehouseReport> Add(WarehouseReportDto warehouseReportDto,
            int hardwareStoreId, int warehouseId)
        {
            var newReport = new WarehouseReport()
            {
                HardwareStoreId = hardwareStoreId,
                WarehouseId = warehouseId,
                Description = warehouseReportDto.Description,
                ReportType = warehouseReportDto.ReportType,
                DateReported = DateTime.Now
            };

            _context.WarehouseReports.Add(newReport);
            await _context.SaveChangesAsync();

            return newReport;
        }
        public async Task<List<WarehouseReport>> GetWarehouseReports(int hardwareStoreId, string reportType)
        {
            var reports = await _context.WarehouseReports.Where(r => r.HardwareStoreId == hardwareStoreId &&
            r.ReportType == reportType)
                .Include(r => r.Warehouse).ToListAsync();

            return reports;
        }

        public async Task<WarehouseReport> GetWarehouseReport(int warehouseReportId, string reportType)
        {
            var report = await _context.WarehouseReports.Where(r => r.Id == warehouseReportId &&
            r.ReportType == reportType).Include(r => r.Warehouse).FirstOrDefaultAsync();

            return report;
        }

        public async Task RemoveReport(int warehouseReportId)
        {
            var reportToDelete = await _context.WarehouseReports.Where(r => r.Id == warehouseReportId)
                .FirstOrDefaultAsync(); 
            if(reportToDelete != null)
            {
                _context.WarehouseReports.Remove(reportToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<WarehouseReport>> GetWarehouseReportsWithWarehouseId(int hardwareStoreId, string reportType, int warehouseId)
        {
            var reports = await _context.WarehouseReports.Where(r => r.HardwareStoreId == hardwareStoreId &&
            r.ReportType == reportType && r.WarehouseId
             == warehouseId)
                .Include(r => r.Warehouse).ToListAsync();

            return reports;
        }
    }
}
