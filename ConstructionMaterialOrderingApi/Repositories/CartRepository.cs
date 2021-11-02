using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.CartDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddToCart(int customerId, AddToCartDto model)
        {
            if(customerId != 0)
            {
                var productInCartIsExist = await _context.Carts
                    .Where(c => c.CustomerId == customerId && c.HardwareStoreId == model.HardwareStoreId && c.ProductId == model.ProductId)
                    .FirstOrDefaultAsync(); 
                if(productInCartIsExist != null)
                {
                    var product = await _context.Products.Where(p => p.Id == model.ProductId && p.HardwareStoreId == model.HardwareStoreId)
                        .FirstOrDefaultAsync();
                    productInCartIsExist.ProductQuantity += 1;
                    if (product.StockNumber >= productInCartIsExist.ProductQuantity)
                    {
                        await _context.SaveChangesAsync();
                        return true;
                    }

                    return false;
                }
                else
                {
                    var cart = new Cart()
                    {
                        CustomerId = customerId,
                        HardwareStoreId = model.HardwareStoreId,
                        ProductId = model.ProductId,
                        CategoryId = model.CategoryId,
                        ProductName = model.ProductName,
                        ProductDescription = model.ProductDescription,
                        ProductBrand = model.ProductBrand,
                        ProductQuality = model.ProductQuality,
                        ProductPrice = model.ProductPrice,
                        ProductQuantity = 1
                    };

                    await _context.Carts.AddAsync(cart);
                    await _context.SaveChangesAsync();

                    return true;
                }

            }

            return false;
        }

        public async Task DecrementQuantity(int customerId, int productId, int hardwareStoreId, int cartId)
        {
            var cart = await _context.Carts
                .Where(c => c.Id == cartId && c.CustomerId == customerId && c.ProductId == productId && c.HardwareStoreId == hardwareStoreId)
                .FirstOrDefaultAsync(); 
            if(cart != null)
            {
                if(cart.ProductQuantity > 1)
                {
                    cart.ProductQuantity -= 1;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<List<GetProductToCartDto>> GetAllProductsToCart(int customerId, int hardwareStoreId)
        {
            var listOfProductsInCart = new List<GetProductToCartDto>();

            var productsInCart = await _context.Carts
                .Where(c => c.CustomerId == customerId && c.HardwareStoreId == hardwareStoreId)
                .ToListAsync();

            productsInCart.ForEach((productInCart) => 
            {
                var productInCartDto = new GetProductToCartDto()
                {
                    CartId = productInCart.Id,
                    HardwareStoreId = productInCart.HardwareStoreId,
                    CustomerId = productInCart.CustomerId,
                    ProductId = productInCart.ProductId,
                    CategoryId = productInCart.CategoryId,
                    ProductName = productInCart.ProductName,
                    ProductDescription = productInCart.ProductDescription,
                    ProductBrand = productInCart.ProductBrand,
                    ProductQuality = productInCart.ProductQuality,
                    ProductPrice = productInCart.ProductPrice,
                    ProductQuantity = productInCart.ProductQuantity
                };
                listOfProductsInCart.Add(productInCartDto);
            });

            return listOfProductsInCart;
        }

        public async Task IncrementQuantity(int customerId, int productId, int hardwareStoreId, int cartId)
        {
            var product = await _context.Products.Where(p => p.Id == productId && p.HardwareStoreId == hardwareStoreId)
                .FirstOrDefaultAsync();
            var cart = await _context.Carts
                .Where(c => c.Id == cartId && c.CustomerId == customerId && c.ProductId == productId && c.HardwareStoreId == hardwareStoreId)
                .FirstOrDefaultAsync(); 

            if(product != null && cart != null)
            {
                cart.ProductQuantity += 1;
                if(product.StockNumber >= cart.ProductQuantity)
                {
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<bool> RemoveToCart(int customerId, int productId, int hardwareStoreId, int cartId)
        {
            if(customerId != 0 && productId != 0 && hardwareStoreId != 0)
            {
                var cart = await _context.Carts
                    .Where(c => c.Id == cartId && c.CustomerId == customerId && c.ProductId == productId && c.HardwareStoreId == hardwareStoreId)
                    .FirstOrDefaultAsync(); 
                if(cart != null)
                {
                    _context.Carts.Remove(cart);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
