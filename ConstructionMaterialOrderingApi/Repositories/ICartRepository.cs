using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.CartDtos;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface ICartRepository
    {
        Task<bool> AddToCart(int customerId,AddToCartDto model, int quantity);
        Task<bool> DeleteCartItem(int cartId, int branchId);
        Task<List<Cart>> ProductsPendingInCart(int branchId);
        Task<List<GetProductToCartDto>> GetAllProductsToCart(int customerId, int hardwareStoreId, int branchId);
        Task<bool> RemoveToCart(int customerId, int productId, int hardwareStoreId, int cartId, int branchId);
        Task IncrementQuantity(int customerId, int productId, int hardwareStoreId, int cartId, int branchId);
        Task DecrementQuantity(int customerId, int productId, int hardwareStoreId, int cartId, int branchId);
        Task<bool> AdjustQuantityWithInput(int customerId, int productId, int hardwareStoreId, int cartId, int branchId, int quantity);
    }
}
