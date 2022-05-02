using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.WarehouseDto;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationRepository _notificationRepository;

        public WarehouseRepository(ApplicationDbContext context, INotificationRepository notificationRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
        }

        public async Task AddWarehouse(WarehouseDto warehouseDto, int hardwareStoreId)
        {
            var newWareHouse = new Warehouse()
            {
                HardwareStoreId = hardwareStoreId,
                Name = warehouseDto.WarehouseName,
                Address = warehouseDto.Address
            };
            _context.Warehouses.Add(newWareHouse);
            await _context.SaveChangesAsync();

            await _notificationRepository.CreateWarehouseNotificationNumber(newWareHouse.Id);
        }
        public async Task<List<Warehouse>> GetWarehouses(int hardwareStoreId)
        {
            var warehouses = await _context.Warehouses.Where(w => w.HardwareStoreId == hardwareStoreId)
                .ToListAsync();
            return warehouses;
        }
        public async Task<Warehouse> GetWarehouse(int warehouseId, int hardwareStoreId)
        {
            var warehouse = await _context.Warehouses
                .Where(w => w.Id == warehouseId && w.HardwareStoreId == hardwareStoreId)
                .FirstOrDefaultAsync();
            return warehouse;
        }
    }
}
