using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.HardwareProductDtos;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Storage;
using System.IO;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class HardwareProductRepository : IHardwareProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationRepository _notificationRepository;
        private readonly IWarehouseProductRepository _warehouseProductRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IBranchRepository _branchRepository;

        public HardwareProductRepository(ApplicationDbContext context, INotificationRepository notificationRepository,
            IWarehouseProductRepository warehouseProductRepository,
            IWarehouseRepository warehouseRepository, IBranchRepository branchRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
            _warehouseProductRepository = warehouseProductRepository;
            _warehouseRepository = warehouseRepository;
            _branchRepository = branchRepository;
        }

        public async Task Add(HardwareProductDto productDto)
        {
            var newProduct = new HardwareProduct()
            {
                HardwareStoreId = productDto.HardwareStoreId,
                Name = productDto.ItemName,
                Description = productDto.Description,
                Supplier = productDto.Supplier,
                CostPrice = productDto.CostPrice,
                CategoryId = productDto.CategoryId,
                //IsActive = productDto.IsActive,
                //AddedAt = DateTime.Now,
                //LastModified = DateTime.Now
            };
            _context.HardwareProducts.Add(newProduct);
            await _context.SaveChangesAsync();

            await _warehouseProductRepository.Add(productDto, newProduct.Id);

            var warehouse = await _warehouseRepository.GetWarehouse(productDto.WarehouseId, productDto.HardwareStoreId);
            await NotifyBranchesNewProductAdded(productDto.HardwareStoreId, warehouse.Name);
            
        }
        private async Task NotifyBranchesNewProductAdded(int hardwareStoreId, string warehouseName)
        {
            var branches = await _branchRepository.GetBranchesByStoreId(hardwareStoreId);

            StringBuilder notificationText = new StringBuilder(warehouseName);
            notificationText.Append(" added new product!");

            // $"{warehouseName} added new product!"
            foreach (var branch in branches)
            {
                await _notificationRepository.PushNotification(branch.Id,
                    notificationText.ToString(), "None");
            }
        }

        public async Task<List<HardwareProduct>> GetProducts(int hardwareStoreId)
        {
            var products = await _context.HardwareProducts
                .Where(p => p.HardwareStoreId == hardwareStoreId)
                .Include(c => c.Category).ToListAsync();
            return products;
        }
        public async Task<HardwareProduct> GetProduct(int hardwareProductId)
        {
            var product = await _context.HardwareProducts
                .Where(p => p.Id == hardwareProductId)
                .Include(c => c.Category)
                .FirstOrDefaultAsync();

            return product;
        }
        private async Task NotifyWarehouseThatHasDeletedProduct(int warehouseId,
            int hardwareStoreId,
            string notificationText)
        {
            var warehouses = await _warehouseRepository.GetWarehouses(hardwareStoreId);

            foreach(var warehouse in warehouses)
            {
                if(warehouse.Id != warehouseId)
                {
                    await _notificationRepository.PushWarehouseNotification(warehouse.Id, notificationText, "None");
                }
            }
        }
        private async Task NotifyDeletedProductInBranches(List<Product> productInBranches, 
            bool oneLeftWarehouse, 
            string warehouseName)
        {
            if(productInBranches != null)
            {
                foreach (var productInBranch in productInBranches)
                {
                    var productToUpdate = await _context.Products
                        .Where(p => p.BranchId == productInBranch.BranchId && p.HardwareProductId == productInBranch.HardwareProductId)
                        .FirstOrDefaultAsync();
                    if (oneLeftWarehouse)
                    {
                        productToUpdate.IsAvailableInWarehouse = false;
                        await _context.SaveChangesAsync();

                        StringBuilder notificationText = new StringBuilder(productToUpdate.Name);
                        notificationText.Append(" is not available in any warehouses, because ");
                        notificationText.Append(warehouseName);
                        notificationText.Append(" only have left this product and it was already deleted.");

                        // $"{productToUpdate.Name} is not available in any warehouses, because {warehouseName} only have left this product and it's already deleted."
                        await _notificationRepository.PushNotification(productToUpdate.BranchId,
                            notificationText.ToString(), "None");
                    }
                    else
                    {
                        StringBuilder notificationText = new StringBuilder(productToUpdate.Name);
                        notificationText.Append(" was already deleted in ");
                        notificationText.Append(warehouseName);

                        // $"{productToUpdate.Name} is already deleted in {warehouseName}"
                        await _notificationRepository.PushNotification(productToUpdate.BranchId,
                            notificationText.ToString(), "None");
                    }
                }
            }
        }
        public async Task<bool> Delete(int hardwareStoreId,
            int hardwareProductId, int warehouseId)
        {
            var product = await _context.HardwareProducts
                .Where(p => p.Id == hardwareProductId && p.HardwareStoreId == hardwareStoreId)
                .FirstOrDefaultAsync();

            if (product != null)
            {
                var warehouse = await _warehouseRepository.GetWarehouse(warehouseId, hardwareStoreId);
                var warehouseProducts = await _warehouseProductRepository.GetProductsByHardwareProductId(hardwareProductId);

                var productInBranches = await _context.Products
                    .Where(p => p.HardwareProductId == hardwareProductId
                    && p.HardwareStoreId == hardwareStoreId).ToListAsync();

                StringBuilder notificationTextToWarehouse = new StringBuilder();

                if(warehouseProducts.Count() == 1)
                {
                    //if (productInBranches != null)
                    //{
                    //    foreach (var productInBranch in productInBranches)
                    //    {
                    //        var productToUpdate = await _context.Products
                    //            .Where(p => p.BranchId == productInBranch.BranchId && p.HardwareProductId == productInBranch.HardwareProductId)
                    //            .FirstOrDefaultAsync();
                    //        productToUpdate.IsAvailableInWarehouse = false;
                    //        await _context.SaveChangesAsync();

                    //        await _notificationRepository.PushNotification(productToUpdate.BranchId,
                    //            $"{productToUpdate.Name} is not available in warehouse because it's already deleted.");
                    //    }

                    //} 
                    await NotifyDeletedProductInBranches(productInBranches, true, warehouse.Name);

                    notificationTextToWarehouse.Append(product.Name);
                    notificationTextToWarehouse.Append(" was deleted in ");
                    notificationTextToWarehouse.Append(warehouse.Name);
                    notificationTextToWarehouse.Append(". The product that was mentioned was no longer available in any warehouses.");

                    await NotifyWarehouseThatHasDeletedProduct(warehouseId, hardwareStoreId, notificationTextToWarehouse.ToString());

                    // if one warehouse left containing the product to be delete, then the original product
                    // will be deleted permanently.

                    var hardwareProductToDelete = await _context.HardwareProducts.Where(p => p.Id == hardwareProductId)
                        .FirstOrDefaultAsync();

                    _context.HardwareProducts.Remove(hardwareProductToDelete);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //TODO : implement notification to warehouse to inform that
                    //this product in warehouse was deleted 

                    //foreach(var warehouseProduct in warehouseProducts)
                    //{
                    //    if(warehouseProduct.WarehouseId != warehouseId)
                    //    {
                    //        await _notificationRepository.PushWarehouseNotification(warehouseProduct.WarehouseId, $"message");
                    //    }
                    //} 
                    await NotifyDeletedProductInBranches(productInBranches, false, warehouse.Name);

                    notificationTextToWarehouse.Append(product.Name);
                    notificationTextToWarehouse.Append(" was deleted in ");
                    notificationTextToWarehouse.Append(warehouse.Name);

                    await NotifyWarehouseThatHasDeletedProduct(warehouseId, hardwareStoreId, notificationTextToWarehouse.ToString());

                }

                await _warehouseProductRepository.Delete(product.Id, warehouseId);
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<string> UploadProductImage(int hardwareProductId, string dbPath,int hardwareStoreId)
        {
            var hardwareProduct = await _context.HardwareProducts.Where(p => p.HardwareStoreId == hardwareStoreId && p.Id == hardwareProductId)
                .FirstOrDefaultAsync();
            if(hardwareProduct != null)
            {
                //string imageUrl = await UploadImageToFirebaseStorage(file, stream);
                hardwareProduct.ImageFile = dbPath;
                await _context.SaveChangesAsync();
                var productsToBeUpdate = await _context.Products.Where(p => p.HardwareProductId == hardwareProductId).ToListAsync();
                foreach (var product in productsToBeUpdate)
                {
                    product.ImageFile = dbPath;

                    await _context.SaveChangesAsync();
                }

                return dbPath;
            }

            return "";
        }
        public async Task<bool> Update(HardwareProductDto productDto, int hardwareProductId,
            int hardwareStoreId)
        {
            var productToUpdate = await _context.HardwareProducts.Where(p => p.Id == hardwareProductId
            && p.HardwareStoreId == hardwareStoreId)
                .FirstOrDefaultAsync();
            if (productToUpdate != null)
            {
                //string imageUrl = UploadImageToFirebaseStorage(productDto.ImageFile);
                productToUpdate.Name = productDto.ItemName;
                productToUpdate.Description = productDto.Description;
                productToUpdate.CategoryId = productDto.CategoryId;
                productToUpdate.Supplier = productDto.Supplier;
                productToUpdate.CostPrice = productDto.CostPrice;
                //productToUpdate.ImageFile = imageUrl;
                //productToUpdate.IsActive = productDto.IsActive;
                //productToUpdate.AddedAt = productDto.AddedAt;
                //productToUpdate.LastModified = DateTime.Now;

                await _warehouseProductRepository.Update(productDto, hardwareProductId);

                var productsToBeUpdate = await _context.Products.Where(p => p.HardwareProductId == hardwareProductId).ToListAsync();
                foreach (var product in productsToBeUpdate)
                {
                    product.Price = (double)productDto.CostPrice;
                    product.Description = productDto.Description;
                    product.CategoryId = productDto.CategoryId;
                    //hjmproduct.ImageFile = imageUrl;

                    await _context.SaveChangesAsync();
                }
                var warehouse = await _warehouseRepository.GetWarehouse(productDto.WarehouseId, hardwareStoreId);
                await NotifyBranchesModifiedProduct(hardwareProductId, hardwareStoreId , productToUpdate.Name, warehouse.Name);
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<string> UploadImageToFirebaseStorage(IFormFile file, FileStream stream)
        {
            if(file.Length > 0)
            {
                //StreamReader streamReader = new StreamReader(file.OpenReadStream());
                var task = new FirebaseStorage("capstone-project-storage.appspot.com", new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                })
                    .Child("ProductImages")
                    .Child(file.FileName)
                    .PutAsync(stream);

                return await task;
            }
            return string.Empty;
        }
        private async Task NotifyBranchesModifiedProduct(int hardwareProductId, 
            int hardwareStoreId, string productName, string warehouseName)
        {
            var branchesHavingProduct = await _context.Products.Where(p => p.HardwareProductId == hardwareProductId
            && p.HardwareStoreId == hardwareStoreId).ToListAsync(); 
            if(branchesHavingProduct != null)
            {
                StringBuilder notificationText = new StringBuilder(warehouseName);
                notificationText.Append(" modified ");
                notificationText.Append(productName);
                // $"{warehouseName} modified {productName}."

                foreach (var branchHavingProduct in branchesHavingProduct)
                {
                    await _notificationRepository.PushNotification(branchHavingProduct.BranchId,
                        notificationText.ToString(), "None");
                }
            }
        }
    }
}
