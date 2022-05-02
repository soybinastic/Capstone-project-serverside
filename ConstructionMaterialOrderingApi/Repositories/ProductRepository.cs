using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.ProductDtos;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IHardwareStoreRepository _harwareStoreRepo;
        private readonly ApplicationDbContext _context;

        public ProductRepository(IHardwareStoreRepository hardwareStoreRepository, ApplicationDbContext context)
        {
            _harwareStoreRepo = hardwareStoreRepository;
            _context = context;
        }
        public async Task<bool> CreateHardwareProduct(int hardwareStoreId, HardwareProduct hardwareProduct, int quantity, int branchId)
        {
            var isExist = await _context.Products.Where(p => p.BranchId == branchId && p.HardwareProductId == hardwareProduct.Id)
                .AnyAsync();
            if (!isExist)
            {
                var product = new Product()
                {
                    HardwareProductId = hardwareProduct.Id,
                    BranchId = branchId,
                    CategoryId = hardwareProduct.CategoryId,
                    HardwareStoreId = hardwareStoreId,
                    Name = hardwareProduct.Name,
                    Description = hardwareProduct.Description,
                    Brand = "None",
                    Quality = "None",
                    Price = (double)hardwareProduct.CostPrice,
                    StockNumber = quantity,
                    IsAvailable = IsAvailableProduct(quantity),
                    IsAvailableInWarehouse = false
                };

                await _context.Products.AddAsync(product);
                _context.SaveChanges();
                return true;
            }

            return false;
            
        } 
        public async Task<Product> GetHardwareProduct(int branchId, int hardwareProductId)
        {
            var product = await _context.Products.Where(p => p.BranchId == branchId && p.HardwareProductId == hardwareProductId)
                .FirstOrDefaultAsync();

            return product;
        }

        public async Task<List<Product>> GetHardwareProducts(int branchId)
        {
            var products = await _context.Products.Where(p => p.BranchId == branchId)
                .ToListAsync();
            return products;
        }
        public async Task<List<Product>> GetHardwareProductByCategory(int branchId, int categoryId)
        {
            var products = await _context.Products.Where(p => p.BranchId == branchId && p.CategoryId == categoryId)
                .ToListAsync();

            return products;
        }

        public async Task<bool> UpdateHardwareProduct(UpdateHardwareProductDto hardwareProductDto, int branchId, int hardwareProductId)
        {
            var isExist = await _context.Products.Where(p => p.BranchId == branchId && p.HardwareProductId == hardwareProductId)
                .AnyAsync();

            if (isExist)
            {
                var productToUpdate = await _context.Products.Where(p => p.BranchId == branchId && p.HardwareProductId == hardwareProductId)
                .FirstOrDefaultAsync();
                productToUpdate.Brand = hardwareProductDto.Brand;
                productToUpdate.Quality = hardwareProductDto.Quality;

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task UpdateQuantity(int branchId, int hardwareProductId, int quantity)
        {
            var isExist = await _context.Products.Where(p => p.BranchId == branchId && p.HardwareProductId == hardwareProductId)
                .AnyAsync();

            if (isExist)
            {
                var productToUpdate = await _context.Products.Where(p => p.BranchId == branchId && p.HardwareProductId == hardwareProductId)
                .FirstOrDefaultAsync();
                productToUpdate.StockNumber = quantity;

                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> CreateProduct(int hardwareStoreId, CreateProductDto model)
        {
            TextInfo text = new CultureInfo("en-US", false).TextInfo;
            var hardwareStore = await _harwareStoreRepo.GetHardwareByStoreId(hardwareStoreId); 
            if(hardwareStore != null)
            {
                model.Name = text.ToTitleCase(model.Name);
                model.Brand = text.ToTitleCase(model.Brand);
                model.Quality = text.ToTitleCase(model.Quality); 

                var isProductExist = await _context.Products
                    .Where(p => p.HardwareStoreId == hardwareStore.HardwareStoreId && p.Name == model.Name
                    && p.Brand == model.Brand && p.Quality == model.Quality).FirstOrDefaultAsync();
                if(isProductExist == null)
                {
                    var product = new Product()
                    {
                        CategoryId = model.CategoryId,
                        HardwareStoreId = hardwareStore.HardwareStoreId,
                        Name = model.Name,
                        Description = model.Description,
                        Brand = model.Brand,
                        Quality = model.Quality,
                        Price = model.Price,
                        StockNumber = model.StockNumber,
                        IsAvailable = IsAvailableProduct(model.StockNumber)
                    };

                    await _context.Products.AddAsync(product);
                    _context.SaveChanges();

                    return true;
                }

                return false;

            }
            return true;
        }

        public async Task<bool> DeleteProduct(int hardwareStoreId, int productId)
        {
            var hardwareStore = await _harwareStoreRepo.GetHardwareByStoreId(hardwareStoreId);
            if(hardwareStore != null)
            {
                var product = await _context.Products
                    .Where(p => p.HardwareStoreId == hardwareStore.HardwareStoreId && p.Id == productId)
                    .FirstOrDefaultAsync();
                if(product != null)
                {
                    _context.Products.Remove(product);
                    _context.SaveChanges();

                    return true;
                }

                return false;
            }

            return false;
        }

        public async Task<List<GetProductDto>> GetAllProduct(int hardwareStoreId)
        {
            var listOfProducts = new List<GetProductDto>();
            var hardwareStore = await _harwareStoreRepo.GetHardwareByStoreId(hardwareStoreId); 
            if(hardwareStore != null)
            {
                var products = await _context.Products.Where(p => p.HardwareStoreId == hardwareStore.HardwareStoreId)
                    .ToListAsync();

                products.ForEach((product) => 
                {
                    var productDto = new GetProductDto()
                    {
                        ProductId = product.Id,
                        HardwareStoreId = product.HardwareStoreId,
                        CategoryId = product.CategoryId,
                        Name = product.Name,
                        Price = product.Price,
                        Description = product.Description,
                        Brand = product.Brand,
                        Quality = product.Quality,
                        StockNumber = product.StockNumber,
                        IsAvailable = product.IsAvailable
                    };

                    listOfProducts.Add(productDto);
                });

                return listOfProducts;
            }

            return listOfProducts;
        }

        public async Task<List<GetProductDto>> GetAllProductByCategory(int hardwareStoreId, int categoryId)
        {
            var listOfProducts = new List<GetProductDto>();
            var hardwareStore = await _harwareStoreRepo.GetHardwareByStoreId(hardwareStoreId); 

            if(hardwareStore != null)
            {
                var products = await _context.Products
                    .Where(p => p.HardwareStoreId == hardwareStore.HardwareStoreId && p.CategoryId == categoryId)
                    .ToListAsync();

                products.ForEach((product) => 
                {
                    var productDto = new GetProductDto()
                    {
                        HardwareStoreId = product.HardwareStoreId,
                        ProductId = product.Id,
                        CategoryId = product.CategoryId,
                        Name = product.Name,
                        Brand = product.Brand,
                        Quality = product.Quality,
                        Description = product.Description,
                        Price = product.Price,
                        StockNumber = product.StockNumber,
                        IsAvailable = product.IsAvailable
                    };
                    listOfProducts.Add(productDto);
                });

                return listOfProducts;
            }
            return listOfProducts;
        }

        public async Task<GetProductDto> GetProduct(int hardwareStoreId, int productId)
        {
            var productDto = new GetProductDto();
            var hardwareStore = await _harwareStoreRepo.GetHardwareByStoreId(hardwareStoreId);
            var product = await _context.Products
                .Where(p => p.HardwareStoreId == hardwareStore.HardwareStoreId && p.Id == productId)
                .FirstOrDefaultAsync(); 
            if(hardwareStore != null && product != null)
            {
                productDto.ProductId = product.Id;
                productDto.CategoryId = product.CategoryId;
                productDto.HardwareStoreId = product.HardwareStoreId;
                productDto.Name = product.Name;
                productDto.Description = product.Description;
                productDto.Brand = product.Brand;
                productDto.Quality = product.Quality;
                productDto.Price = product.Price;
                productDto.StockNumber = product.StockNumber;
                productDto.IsAvailable = product.IsAvailable;

                return productDto;
            }
            return productDto;
        }

        public async Task<bool> UpdateProduct(int hardwareStoreId, int productId, UpdateProductDto model)
        {
            if(productId == model.ProductId)
            {
                TextInfo text = new CultureInfo("en-US", false).TextInfo;
                model.Name = text.ToTitleCase(model.Name);
                model.Brand = text.ToTitleCase(model.Brand);
                model.Quality = text.ToTitleCase(model.Quality);

                var isProductExist = await _context.Products
                    .Where(p => p.HardwareStoreId == hardwareStoreId && p.Name == model.Name && p.Brand == model.Brand && p.Quality == model.Quality 
                    && p.Id != model.ProductId)
                    .FirstOrDefaultAsync();
               if(isProductExist == null)
                {
                    var hardwareStore = await _harwareStoreRepo.GetHardwareByStoreId(hardwareStoreId);
                    var product = await _context.Products
                        .Where(p => p.HardwareStoreId == hardwareStore.HardwareStoreId && p.Id == productId)
                        .FirstOrDefaultAsync();

                    if (hardwareStore != null && product != null)
                    {
                        product.Name = model.Name;
                        product.CategoryId = model.CategoryId;
                        product.Price = model.Price;
                        product.Description = model.Description;
                        product.Brand = model.Brand;
                        product.Quality = model.Quality;
                        product.StockNumber = model.StockNumber;
                        product.IsAvailable = IsAvailableProduct(model.StockNumber);

                        _context.SaveChanges();
                        return true;
                    }

                    return false;
                }

                return false;
            }

            return false;
        }

        private bool IsAvailableProduct(int stockNumber)
        {
            if(stockNumber > 0)
            {
                return true;
            }
            return false;
        }
    }
}
