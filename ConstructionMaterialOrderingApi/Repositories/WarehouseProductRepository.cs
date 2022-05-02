using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.HardwareProductDtos;
using ConstructionMaterialOrderingApi.Dtos.MoveProductDto;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class WarehouseProductRepository : IWarehouseProductRepository
    {
        private readonly ApplicationDbContext _context;

        public WarehouseProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(HardwareProductDto productDto, int hardwareProductId)
        {
            var newWarehouseProduct = new WarehouseProduct()
            {
                WarehouseId = productDto.WarehouseId,
                HardwareProductId = hardwareProductId,
                StockNumber = productDto.StockNumber,
                IsActive = productDto.IsActive,
                AddedAt = DateTime.Now,
                LastModified = DateTime.Now
            };

            _context.WarehouseProducts.Add(newWarehouseProduct);
            await _context.SaveChangesAsync();
        }
        public async Task<WarehouseProduct> GetProduct(int warehouseId, int hardwareProductId)
        {
            var warehouseProduct = await _context.WarehouseProducts
                .Where(wp => wp.WarehouseId == warehouseId && wp.HardwareProductId == hardwareProductId)
                .Include(wp => wp.HardwareProduct)
                .ThenInclude(wp => wp.Category)
                .FirstOrDefaultAsync();
            return warehouseProduct;
        }
        public async Task<List<WarehouseProduct>> GetProducts(int warehouseId)
        {
            var warehouseProducts = await _context.WarehouseProducts
                .Where(wp => wp.WarehouseId == warehouseId)
                .Include(wp => wp.HardwareProduct)
                .ThenInclude(wp => wp.Category)
                .OrderByDescending(p => p.LastModified)
                .ToListAsync();

            return warehouseProducts;
        }
        public async Task Update(HardwareProductDto productDto, int hardwareProductId)
        {
            var warehouseProductToUpdate = await _context.WarehouseProducts
                .Where(wp => wp.HardwareProductId == hardwareProductId
                && wp.WarehouseId == productDto.WarehouseId)
                .FirstOrDefaultAsync();
            if (warehouseProductToUpdate != null)
            {
                warehouseProductToUpdate.StockNumber = productDto.StockNumber;
                warehouseProductToUpdate.IsActive = productDto.IsActive;
                warehouseProductToUpdate.LastModified = DateTime.Now;
                warehouseProductToUpdate.AddedAt = productDto.AddedAt;
                await _context.SaveChangesAsync();
            }

            
        } 
        public async Task Delete(int hardwareProductId, int warehouseId)
        {
            //var warehouseProductToDelete = await _context.WarehouseProducts
            //    .Where(p => p.HardwareProductId == hardwareProductId)
            //    .ToListAsync(); 
            //if(warehouseProductToDelete != null)
            //{
            //    foreach(var product in warehouseProductToDelete)
            //    {
            //        _context.WarehouseProducts.Remove(product);
            //        await _context.SaveChangesAsync();
            //    }
            //} 
            var warehouseProductToDelete = await _context.WarehouseProducts
                .Where(wp => wp.HardwareProductId == hardwareProductId && wp.WarehouseId == warehouseId)
                .FirstOrDefaultAsync(); 
            if(warehouseProductToDelete != null)
            {
                _context.WarehouseProducts.Remove(warehouseProductToDelete);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> MoveProductToWarehouse(MoveProductDto moveProductDto, int hardwareStoreId)
        {
            var product = await _context.HardwareProducts
                .Where(hp => hp.Id == moveProductDto.HardwareProductId)
                .FirstOrDefaultAsync();

            var fromWarehouse = await _context.WarehouseProducts
                .Where(wp => wp.Id == moveProductDto.FromWarehouseId)
                .FirstOrDefaultAsync();

            var toWarehouse = await _context.WarehouseProducts
                .Where(wp => wp.Id == moveProductDto.MoveToWarehouseId)
                .FirstOrDefaultAsync();

            if (product != null && fromWarehouse != null && toWarehouse != null)
            {
                var moveProduct = await _context.MoveProducts
                    .Where(mp => mp.HardwareProductId == moveProductDto.HardwareProductId
                    && mp.MoveToWarehouseId == moveProductDto.MoveToWarehouseId 
                    && mp.FromWarehouseId == moveProductDto.FromWarehouseId)
                    .FirstOrDefaultAsync();

                var newMoveProduct = new MoveProduct()
                {
                    HardwareStoreId = hardwareStoreId,
                    HardwareProductId = moveProductDto.HardwareProductId,
                    FromWarehouseId = moveProductDto.FromWarehouseId,
                    MoveToWarehouseId = moveProductDto.MoveToWarehouseId,
                    Quantity = moveProductDto.Quantity,
                    MoveDate = DateTime.Now
                };

                if (moveProduct == null)
                {
                    _context.MoveProducts.Add(newMoveProduct);
                    await Add(new HardwareProductDto { WarehouseId = moveProductDto.MoveToWarehouseId, StockNumber = moveProductDto.Quantity },
                        moveProductDto.HardwareProductId);
                }
                else
                {
                    _context.MoveProducts.Add(newMoveProduct);
                    await Update(new HardwareProductDto { WarehouseId = moveProductDto.MoveToWarehouseId }
                    , moveProductDto.HardwareProductId);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<WarehouseProduct>> GetAvailableProducts(int warehouseId)
        {
            var availableProducts = await _context.WarehouseProducts
                .Where(p => p.WarehouseId == warehouseId && p.StockNumber > 0)
                .Include(p => p.HardwareProduct)
                .ThenInclude(p => p.Category)
                .ToListAsync();
            return availableProducts;
        }

        public async Task<List<WarehouseProduct>> GetProductsByHardwareProductId(int hardwareProductId)
        {
            var products = await _context.WarehouseProducts
                .Where(p => p.HardwareProductId == hardwareProductId)
                .ToListAsync();
            return products;
        }
    }
}
