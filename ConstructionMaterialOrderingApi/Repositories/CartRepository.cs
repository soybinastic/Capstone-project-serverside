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
        public async Task<bool> DeleteCartItem(int cartId, int branchId)
        {
            var cartItem = await _context.Carts.Where(c => c.Id == cartId && c.BranchId == branchId)
                    .FirstOrDefaultAsync(); 
            if(cartItem != null)
            {
                var product = await _context.Products.Where(p => p.HardwareProductId == cartItem.ProductId && p.BranchId == branchId)
                        .FirstOrDefaultAsync(); 
                if(product != null)
                {
                    product.StockNumber += cartItem.ProductQuantity;
                }

                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<List<Cart>> ProductsPendingInCart(int branchId)
        {
            var productsPendingInCart = await _context.Carts.Where(c => c.BranchId == branchId)
                    .ToListAsync();
            return productsPendingInCart;
        }
        public async Task<bool> AddToCart(int customerId, AddToCartDto model)
        {
            if(customerId != 0)
            {
                var productInCartIsExist = await _context.Carts
                    .Where(c => c.CustomerId == customerId && c.HardwareStoreId == model.HardwareStoreId && c.ProductId == model.ProductId && c.BranchId == model.BranchId)
                    .FirstOrDefaultAsync();

                var product = await _context.Products.Where(p => p.HardwareProductId == model.ProductId && p.HardwareStoreId == model.HardwareStoreId && p.BranchId == model.BranchId)
                        .FirstOrDefaultAsync();
                
                if(product.StockNumber > 0)
                {
                    if (productInCartIsExist != null)
                    {

                        productInCartIsExist.ProductQuantity += 1;
                        product.StockNumber -= 1;

                        await _context.SaveChangesAsync();
                        //if (product.StockNumber >= productInCartIsExist.ProductQuantity)
                        //{
                        //    product.StockNumber -= 1;

                        //    await _context.SaveChangesAsync();
                        //    return true;
                        //}

                        return true;
                    }
                    else
                    {
                        var cart = new Cart()
                        {
                            CustomerId = customerId,
                            HardwareStoreId = model.HardwareStoreId,
                            BranchId = model.BranchId,
                            ProductId = model.ProductId,
                            CategoryId = model.CategoryId,
                            ProductName = model.ProductName,
                            ProductDescription = model.ProductDescription,
                            ProductBrand = model.ProductBrand,
                            ProductQuality = model.ProductQuality,
                            ProductPrice = model.ProductPrice,
                            ProductQuantity = 1,
                            DateAddedToCart = DateTime.Now
                        };

                        product.StockNumber -= 1;

                        await _context.Carts.AddAsync(cart);
                        await _context.SaveChangesAsync();

                        return true;
                    }
                }

                return false;

            }

            return false;
        }

        public async Task DecrementQuantity(int customerId, int productId, int hardwareStoreId, int cartId, int branchId)
        {
            var product = await _context.Products.Where(p => p.HardwareProductId == productId && p.HardwareStoreId == hardwareStoreId && p.BranchId == branchId)
                    .FirstOrDefaultAsync();
            var cart = await _context.Carts
                .Where(c => c.Id == cartId && c.CustomerId == customerId && c.ProductId == productId && c.HardwareStoreId == hardwareStoreId && c.BranchId == branchId)
                .FirstOrDefaultAsync(); 
            if(product != null && cart != null)
            {
                if(cart.ProductQuantity > 1)
                {
                    cart.ProductQuantity -= 1;
                    product.StockNumber += 1;

                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<bool> AdjustQuantityWithInput(int customerId, int productId, int hardwareStoreId, int cartId, int branchId, int quantity)
        {
            var product = await _context.Products.Where(p => p.HardwareProductId == productId && p.HardwareStoreId == hardwareStoreId &&
                p.BranchId == branchId).FirstOrDefaultAsync();
            var cart = await _context.Carts.Where(c => c.Id == cartId && c.CustomerId == customerId && c.ProductId == productId && c.HardwareStoreId == hardwareStoreId && c.BranchId == branchId)
                    .FirstOrDefaultAsync();

            if(product != null && cart != null && quantity > 0)
            {
                if(cart.ProductQuantity < quantity)
                {
                    int difference = quantity - cart.ProductQuantity;
                    int productStockNumber = product.StockNumber - difference;
                   

                    if((productStockNumber + cart.ProductQuantity + difference) >= quantity)
                    {
                        product.StockNumber = productStockNumber;
                        cart.ProductQuantity += difference;

                        await _context.SaveChangesAsync();
                        return true;
                    }

                    return false;
                    
                }
                else if(cart.ProductQuantity > quantity)
                {
                    quantity = cart.ProductQuantity - quantity;
                    product.StockNumber += quantity;
                    cart.ProductQuantity -= quantity;

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            return false;
        }

        public async Task<List<GetProductToCartDto>> GetAllProductsToCart(int customerId, int hardwareStoreId, int branchId)
        {
            var listOfProductsInCart = new List<GetProductToCartDto>();

            var productsInCart = await _context.Carts
                .Where(c => c.CustomerId == customerId && c.HardwareStoreId == hardwareStoreId && c.BranchId == branchId)
                .ToListAsync();

            productsInCart.ForEach((productInCart) => 
            {
                var productInCartDto = new GetProductToCartDto()
                {
                    CartId = productInCart.Id,
                    HardwareStoreId = productInCart.HardwareStoreId,
                    CustomerId = productInCart.CustomerId,
                    ProductId = productInCart.ProductId,
                    BranchId = productInCart.BranchId,
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

        public async Task IncrementQuantity(int customerId, int productId, int hardwareStoreId, int cartId, int branchId)
        {
            var product = await _context.Products.Where(p => p.HardwareProductId == productId && p.HardwareStoreId == hardwareStoreId && p.BranchId == branchId)
                .FirstOrDefaultAsync();
            var cart = await _context.Carts
                .Where(c => c.Id == cartId && c.CustomerId == customerId && c.ProductId == productId && c.HardwareStoreId == hardwareStoreId && c.BranchId == branchId)
                .FirstOrDefaultAsync(); 

            if(product != null && cart != null)
            {
                cart.ProductQuantity += 1;
                if(product.StockNumber > 0)
                {
                    product.StockNumber -= 1;
                    await _context.SaveChangesAsync();
                }
                //if(product.StockNumber >= cart.ProductQuantity)
                //{
                //    product.StockNumber -= 1;

                //    await _context.SaveChangesAsync();
                //}
            }
        }

        public async Task<bool> RemoveToCart(int customerId, int productId, int hardwareStoreId, int cartId, int branchId)
        {
            if(customerId != 0 && productId != 0 && hardwareStoreId != 0)
            {
                var product = await _context.Products.Where(p => p.HardwareProductId == productId && p.HardwareStoreId == hardwareStoreId && p.BranchId == branchId)
                        .FirstOrDefaultAsync();
                var cart = await _context.Carts
                    .Where(c => c.Id == cartId && c.CustomerId == customerId && c.ProductId == productId && c.HardwareStoreId == hardwareStoreId && c.BranchId == branchId)
                    .FirstOrDefaultAsync(); 
                if(product != null && cart != null)
                {
                    product.StockNumber += cart.ProductQuantity; 

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
