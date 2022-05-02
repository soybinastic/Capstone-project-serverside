using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.WarehouseProductStatusReportDto;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class ProductStatusReportRepository : IProductStatusReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductStatusReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(ProductStatusReportDto statusReportDto, int warehouseReportId)
        {
            var newStatusReport = new WarehouseProductStatusReport()
            {
                WarehouseReportId = warehouseReportId,
                HardwareProductId = statusReportDto.HardwareProductId,
                Description = statusReportDto.Description,
                StatusType = statusReportDto.StatusType
            };
            _context.WarehouseProductStatusReports.Add(newStatusReport);
            await _context.SaveChangesAsync();

        }

        public async Task<List<WarehouseProductStatusReport>> GetWarehouseProductStatusReports(int warehouseReportId)
        {
            var productStatusReports = await _context.WarehouseProductStatusReports
                .Where(p => p.WarehouseReportId == warehouseReportId)
                .Include(p => p.HardwareProduct)
                .Include(p => p.WarehouseReport)
                .ThenInclude(w => w.Warehouse)
                .ToListAsync();

            return productStatusReports;
        }

        public async Task Remove(int warehouseProductStatusReportId)
        {
            var warehouseProductReport = await _context.WarehouseProductStatusReports
                .Where(p => p.Id == warehouseProductStatusReportId)
                .FirstOrDefaultAsync();

            if (warehouseProductReport != null)
            {
                _context.WarehouseProductStatusReports.Remove(warehouseProductReport);
                await _context.SaveChangesAsync();
            }
        }
    }
}
