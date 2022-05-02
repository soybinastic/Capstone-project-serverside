using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.HardwareProductDtos;
using ConstructionMaterialOrderingApi.Dtos.MoveProductDto;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class MoveProductRepository : IMoveProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWarehouseProductRepository _warehouseProductRepository;
        private readonly INotificationRepository _notificationRepository;

        public MoveProductRepository(ApplicationDbContext context, IWarehouseProductRepository warehouseProductRepository, 
            INotificationRepository notificationRepository)
        {
            _context = context;
            _warehouseProductRepository = warehouseProductRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<bool> MoveProductToWarehouse(MoveProductDto moveProductDto, int hardwareStoreId)
        {
            var product = await _context.HardwareProducts
                .Where(hp => hp.Id == moveProductDto.HardwareProductId)
                .FirstOrDefaultAsync();

            

            //var toWarehouse = await _context.WarehouseProducts
            //    .Where(wp => wp.Id == moveProductDto.MoveToWarehouseId)
            //    .FirstOrDefaultAsync();

            var warehouseTransferingProdcut = await _context.Warehouses.Where(w => w.Id == moveProductDto.FromWarehouseId)
                .FirstOrDefaultAsync();
            var warehouseRecievingProduct = await _context.Warehouses.Where(w => w.Id == moveProductDto.MoveToWarehouseId)
                .FirstOrDefaultAsync();


            if (product != null && warehouseTransferingProdcut != null && warehouseRecievingProduct != null)
            {
                var fromWarehouseProduct = await _context.WarehouseProducts
                    .Where(wp => wp.WarehouseId == moveProductDto.FromWarehouseId &&
                    wp.HardwareProductId == moveProductDto.HardwareProductId)
                    .Include(wp => wp.HardwareProduct)
                    .FirstOrDefaultAsync();

                if (fromWarehouseProduct != null && moveProductDto.Quantity > 0 && moveProductDto.Quantity <= fromWarehouseProduct.StockNumber)
                {
                    //var moveProduct = await _context.MoveProducts
                    //    .Where(mp => mp.HardwareProductId == moveProductDto.HardwareProductId
                    //    && mp.MoveToWarehouseId == moveProductDto.MoveToWarehouseId
                    //    && mp.FromWarehouseId == moveProductDto.FromWarehouseId)
                    //    .FirstOrDefaultAsync(); 

                    var isThisProductExistInOtherWarehouse = await _context.WarehouseProducts
                        .AnyAsync(wp => wp.WarehouseId == moveProductDto.MoveToWarehouseId 
                        && wp.HardwareProductId == moveProductDto.HardwareProductId);

                    var newMoveProduct = new MoveProduct()
                    {
                        HardwareStoreId = hardwareStoreId,
                        HardwareProductId = moveProductDto.HardwareProductId,
                        FromWarehouseId = moveProductDto.FromWarehouseId,
                        MoveToWarehouseId = moveProductDto.MoveToWarehouseId,
                        Quantity = moveProductDto.Quantity,
                        MoveDate = DateTime.Now
                    };

                    fromWarehouseProduct.StockNumber -= moveProductDto.Quantity;

                    if (!isThisProductExistInOtherWarehouse)
                    {
                        _context.MoveProducts.Add(newMoveProduct);
                        await _warehouseProductRepository
                            .Add(new HardwareProductDto { WarehouseId = moveProductDto.MoveToWarehouseId, StockNumber = moveProductDto.Quantity },
                            moveProductDto.HardwareProductId);

                        StringBuilder notificationText = new StringBuilder(warehouseTransferingProdcut.Name);
                        notificationText.Append(" transferred new product (");
                        notificationText.Append(fromWarehouseProduct.HardwareProduct.Name);
                        notificationText.Append("). Please check the storage!");

                        // $"{warehouseTransferingProdcut.Name} transferred new product ({fromWarehouseProduct.HardwareProduct.Name}). Please check the storage!"

                        await _notificationRepository.PushWarehouseNotification(moveProductDto.MoveToWarehouseId, notificationText.ToString(), "MoveProduct");
                    }
                    else
                    {
                        var warehouseRecieving = await _context.WarehouseProducts.Where(wp => wp.WarehouseId == moveProductDto.MoveToWarehouseId
                        && wp.HardwareProductId == moveProductDto.HardwareProductId)
                            .FirstOrDefaultAsync();

                        _context.MoveProducts.Add(newMoveProduct);

                        warehouseRecieving.StockNumber += moveProductDto.Quantity;
                        await _context.SaveChangesAsync();

                        StringBuilder notificationText = new StringBuilder(warehouseTransferingProdcut.Name);
                        notificationText.Append(" transferred product (");
                        notificationText.Append(fromWarehouseProduct.HardwareProduct.Name);
                        notificationText.Append("). Please check the storage!");

                        // $"{warehouseTransferingProdcut.Name} transferred product ({fromWarehouseProduct.HardwareProduct.Name})"
                        await _notificationRepository.PushWarehouseNotification(moveProductDto.MoveToWarehouseId, notificationText.ToString(), "MoveProduct");
                        //await _warehouseProductRepository
                        //    .Update(new HardwareProductDto { WarehouseId = moveProductDto.MoveToWarehouseId, StockNumber = moveProductDto.Quantity }
                        //, moveProductDto.HardwareProductId);
                    }

                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<TransferProductDto>> GetMoveProducts(int warehouseId)
        {
            var moveProducts = await _context.MoveProducts
                .Where(mp => mp.FromWarehouseId == warehouseId)
                .Include(mp => mp.Product)
                .ToListAsync();

            var moveProductsDto = moveProducts.Select(mp => new TransferProductDto()
                {
                    Id = mp.Id,
                    HardwareStoreId = mp.HardwareStoreId,
                    HardwareProductId = mp.HardwareProductId,
                    Product = mp.Product,
                    FromWarehouseId = mp.FromWarehouseId,
                    MoveToWarehouseId = mp.MoveToWarehouseId,
                    Quantity = mp.Quantity,
                    MoveDate = mp.MoveDate,
                    FromWarehouse = _context.Warehouses.Where(w => w.Id == mp.FromWarehouseId)
                        .FirstOrDefault(),
                    ToWarehouse = _context.Warehouses.Where(w => w.Id == mp.MoveToWarehouseId)
                        .FirstOrDefault()   
                }).OrderByDescending(mp => mp.MoveDate)
                .ToList();

            return moveProductsDto;
        }
    }
}
