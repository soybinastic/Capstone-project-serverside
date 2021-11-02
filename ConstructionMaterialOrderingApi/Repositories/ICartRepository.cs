using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.CartDtos;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface ICartRepository
    {
        Task<bool> AddToCart(int customerId,AddToCartDto model);
        Task<List<GetProductToCartDto>> GetAllProductsToCart(int customerId, int hardwareStoreId);
        Task<bool> RemoveToCart(int customerId, int productId, int hardwareStoreId, int cartId);
        Task IncrementQuantity(int customerId, int productId, int hardwareStoreId, int cartId);
        Task DecrementQuantity(int customerId, int productId, int hardwareStoreId, int cartId);
    }
}
