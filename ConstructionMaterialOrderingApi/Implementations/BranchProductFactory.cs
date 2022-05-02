using System;
using System.Collections.Generic;
using ConstructionMaterialOrderingApi.Dtos.DeliverProductDtos;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using System.Text;
using ConstructionMaterialOrderingApi.Context;
using Microsoft.EntityFrameworkCore;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public class BranchProductFactory : IBranchProductFactory
    {
        private readonly IProductRepository _productRepository;
        private readonly IHardwareProductRepository _hardwareProductRepository;
        private readonly IDeliverProductReportRepository _deliverProductReportRepository;
        private readonly IWarehouseProductRepository _warehouseProductRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IRequestProductRepository _requestProductRepository;
        private readonly ApplicationDbContext _context;

        public BranchProductFactory(IProductRepository productRepository,
            IHardwareProductRepository hardwareProductRepository,
            IDeliverProductReportRepository deliverProductReportRepository,
            IWarehouseProductRepository warehouseProductRepository,
            INotificationRepository notificationRepository,
            IWarehouseRepository warehouseRepository, 
            IRequestRepository requestRepository,
            IRequestProductRepository requestProductRepository,
            ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _hardwareProductRepository = hardwareProductRepository;
            _deliverProductReportRepository = deliverProductReportRepository;
            _warehouseProductRepository = warehouseProductRepository;
            _notificationRepository = notificationRepository;
            _warehouseRepository = warehouseRepository;
            _requestRepository = requestRepository;
            _requestProductRepository = requestProductRepository;
            _context = context;
        }

        public async Task<bool> DeliverProduct(int warehouseId, int hardwareStoreId, DeliverProductDto deliverProductDto)
        {
            var warehouseProducts = await _warehouseProductRepository.GetProducts(warehouseId);
            var warehouseProduct = warehouseProducts.Where(wp => wp.HardwareProductId == deliverProductDto.HardwareProductId)
                .FirstOrDefault();

            if(deliverProductDto.Quantity > 0)
            {
                if (warehouseProduct != null && warehouseProduct.StockNumber >= deliverProductDto.Quantity && warehouseProduct.StockNumber > 0 && warehouseProduct.IsActive)
                {
                    var hardwareProduct = await _hardwareProductRepository.GetProduct(warehouseProduct.HardwareProductId);
                    

                    var warehouse = await _warehouseRepository.GetWarehouse(warehouseId, hardwareStoreId);

                    StringBuilder notificationText = new StringBuilder();
                    

                    if (deliverProductDto.RequestProductId > 0)
                    {
                        var isComplete = await _context.RequestProducts.AnyAsync(rp => rp.Id == deliverProductDto.RequestProductId && rp.IsComplete);
                        if(!isComplete)
                        {
                            notificationText.Append(warehouse.Name);
                            notificationText.Append(" delivered your product request ");
                            notificationText.Append(hardwareProduct.Name);

                            var requestProductNotComplete = await _context.RequestProducts.Where(rp => rp.Id == deliverProductDto.RequestProductId).FirstOrDefaultAsync();
                            await _requestProductRepository.MakeCompleteRequestProduct(requestProductNotComplete.HardwareProductId, requestProductNotComplete.Id);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var requests = await GetRequestProducts(warehouseId, deliverProductDto.BranchId);
                        var requestProductNotComplete = FindRequestProductNotComplete(requests, deliverProductDto.HardwareProductId);

                        if(requestProductNotComplete != null)
                        {
                            notificationText.Append(warehouse.Name);
                            notificationText.Append(" delivered your product request ");
                            notificationText.Append(hardwareProduct.Name);
                            await _requestProductRepository.MakeCompleteRequestProduct(requestProductNotComplete.HardwareProductId, requestProductNotComplete.Id);
                        }
                        else
                        {
                            notificationText.Append(warehouse.Name);
                            notificationText.Append(" deliver a product '");
                            notificationText.Append(hardwareProduct.Name);
                            notificationText.Append("' with quantity of ");
                            notificationText.Append(deliverProductDto.Quantity);
                        }
                    }

                    var deliverProduct = await _productRepository.CreateHardwareProduct(hardwareStoreId, hardwareProduct, deliverProductDto.Quantity, deliverProductDto.BranchId);

                    if (!deliverProduct)
                    {
                        var branchProduct = await _productRepository.GetHardwareProduct(deliverProductDto.BranchId, hardwareProduct.Id);
                        branchProduct.StockNumber += deliverProductDto.Quantity;

                        await _productRepository.UpdateQuantity(deliverProductDto.BranchId, hardwareProduct.Id, branchProduct.StockNumber);
                    }

                    var warehouseProductToUpdate = await _context.WarehouseProducts.Where(wp => wp.WarehouseId == warehouseId && wp.HardwareProductId == deliverProductDto.HardwareProductId)
                        .FirstOrDefaultAsync();

                    warehouseProductToUpdate.StockNumber -= deliverProductDto.Quantity;
                    await _context.SaveChangesAsync();

                    await _deliverProductReportRepository.Add(deliverProductDto, warehouseId, hardwareStoreId, warehouseProduct.Id);

                    await _notificationRepository.PushNotification(deliverProductDto.BranchId, notificationText.ToString(),"DeliverProduct");
                    return true;
                }
                return false;
            }

            return false;
        } 

        private RequestProduct FindRequestProductNotComplete(List<RequestProduct> requestProducts, int hardwareProductId)
        {
            var requestProductNotComplete = requestProducts.Where(rp => rp.HardwareProductId == hardwareProductId && rp.IsComplete == false)
                .FirstOrDefault();
            return requestProductNotComplete;
        }

        private async Task<List<RequestProduct>> GetRequestProducts(int warehouseId, int branchId)
        {
            List<RequestProduct> requestProducts = new List<RequestProduct>();
            var requests = await _requestRepository.GetAllRequestsByWarehouseId(warehouseId);
            requests = requests.Where(rp => rp.BranchId == branchId).ToList();

            foreach(var request in requests)
            {
                requestProducts.AddRange(request.RequestProducts);
            }

            return requestProducts;
        }
    }
}
