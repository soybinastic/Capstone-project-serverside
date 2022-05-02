using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.RecieveProductDtos;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class RecieveProductRepository : IRecieveProductRepository
    {
        private readonly ApplicationDbContext _context;

        public RecieveProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(RecieveProductDto recieveProductDto, int warehouseReportId, int warehouseId)
        {
            var warehouseProduct = await _context.WarehouseProducts.Where(wp => wp.WarehouseId == warehouseId
                && wp.HardwareProductId == recieveProductDto.HardwareProductId).FirstOrDefaultAsync();

            if (warehouseProduct != null)
            {
                var newRecieveProduct = new RecieveProduct()
                {
                    WarehouseReportId = warehouseReportId,
                    HardwareProductId = recieveProductDto.HardwareProductId,
                    Quantity = recieveProductDto.Quantity,
                    CostPrice = recieveProductDto.CostPrice,
                    TotalCost = CalculateTotalCost(recieveProductDto.CostPrice, recieveProductDto.Quantity),
                    DateRecieve = DateTime.Now
                };

                _context.RecieveProducts.Add(newRecieveProduct);
                //await _context.SaveChangesAsync();

                warehouseProduct.StockNumber += recieveProductDto.Quantity;

                await _context.SaveChangesAsync();
            }
        }
        public async Task Update(RecieveProductDto recieveProductDto, int recieveProductId, int warehouseId)
        {
            var recieveProduct = await _context.RecieveProducts.Where(rp => rp.Id == recieveProductId
            && rp.HardwareProductId == recieveProductDto.HardwareProductId)
                .FirstOrDefaultAsync();
            var warehouseProduct = await _context.WarehouseProducts.Where(wp => wp.WarehouseId == warehouseId
            && wp.HardwareProductId == recieveProductDto.HardwareProductId).FirstOrDefaultAsync();

            if (recieveProduct != null && warehouseProduct != null)
            {
                warehouseProduct.StockNumber = (warehouseProduct.StockNumber - recieveProduct.Quantity) + recieveProductDto.Quantity;

                recieveProduct.Quantity = recieveProductDto.Quantity;
                recieveProduct.CostPrice = recieveProductDto.CostPrice;
                recieveProduct.TotalCost = CalculateTotalCost(recieveProductDto.CostPrice, recieveProductDto.Quantity);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<RecieveProduct>> GetRecieveProducts(int warehouseReportId)
        {
            var recieveProducts = await _context.RecieveProducts
                .Where(rp => rp.WarehouseReportId == warehouseReportId)
                .Include(rp => rp.HardwareProduct)
                .ToListAsync();
            return recieveProducts;
        }
        public async Task<RecieveProduct> GetRecieveProduct(int recieveProductId)
        {
            var recieveProduct = await _context.RecieveProducts
                .Where(rp => rp.Id == recieveProductId)
                .Include(rp => rp.HardwareProduct)
                .FirstOrDefaultAsync();
            return recieveProduct;
        }

        private decimal CalculateTotalCost(decimal costPrice, int quantity)
            => quantity * costPrice;
    }
}
