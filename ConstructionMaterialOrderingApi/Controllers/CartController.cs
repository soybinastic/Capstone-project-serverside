using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.CartDtos;
using ConstructionMaterialOrderingApi.Repositories;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IStoreAdminRepository _storeAdminRepository;

        public CartController(ICustomerRepository customerRepository, ICartRepository cartRepository, IStoreAdminRepository storeAdminRepository)
        {
            _customerRepository = customerRepository;
            _cartRepository = cartRepository;
            _storeAdminRepository = storeAdminRepository;
        } 

        [HttpPost]
        [Route("/api/cart/add-to-cart")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto model)
        {
            var customerUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(customerUserAccountId);

            var result = await _cartRepository.AddToCart(customer.CustomerId, model);

            return result ? Ok(new { Success = 1, Message = "Successfully added to cart"}) : BadRequest(new { Succes = 0, Message = "Failed to add"});
        }  

        [HttpGet]
        [Route("/api/cart/get-products-in-cart/{storeId}/{branchId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAllProductsInCart([FromRoute]int storeId, [FromRoute]int branchId)
        {
            var customerUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(customerUserAccountId);

            var productsInCart = await _cartRepository.GetAllProductsToCart(customer.CustomerId, storeId, branchId);

            var productsInCartJsonObj = JsonConvert.SerializeObject(productsInCart, new JsonSerializerSettings 
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return Ok(productsInCartJsonObj);
        }
        [HttpGet]
        [Route("/api/cart/get-products-pending-in-cart")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetAllProductsPendingInCart()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var cartIemsPending = await _cartRepository.ProductsPendingInCart(storeAdmin.BranchId);
            var cartItemsPendingJsonObj = JsonConvert.SerializeObject(cartIemsPending, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return Ok(cartItemsPendingJsonObj);
        } 

        [HttpDelete]
        [Route("/api/cart/delete-product-pending-in-cart/{cartId}")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> DeletePendingProductInCart([FromRoute]int cartId)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var result = await _cartRepository.DeleteCartItem(cartId, storeAdmin.BranchId);
            return result ? Ok(new { Success = 1, Message = "Deleted Successfully." }) : BadRequest(new { Success = 0, Message = "Failed to delete."});
        }

        [HttpDelete]
        [Route("/api/cart/remove-to-cart/{storeId}/{cartId}/{productId}/{branchId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveToCart(int storeId, int cartId, int productId, int branchId)
        {
            var customerUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(customerUserAccountId);
            var result = await _cartRepository.RemoveToCart(customer.CustomerId, productId, storeId, cartId, branchId);

            return result ? Ok(new { Success = 1, Message = "Remove successfully"}) : BadRequest(new { Success = 0, Message = "Failed to remove"});
        }
        
        [HttpPut]
        [Route("/api/cart/adjust-quantity/{storeId}/{cartId}/{productId}/{branchId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AdjustQuantity([FromBody]AdjustQuantityDto quantityDto, int storeId, int cartId, int productId, int branchId)
        {
            var customerUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(customerUserAccountId);
            var result = await _cartRepository.AdjustQuantityWithInput(customer.CustomerId, productId, storeId, cartId, branchId, quantityDto.Quantity);

            return result ? Ok(new { Success = 1, Message = "Successfully adjusted quantity." }) : BadRequest(new { Success = 0, Message = "Failed to adjust quantity" });
        }

        [HttpPut]
        [Route("/api/cart/increment-quantity/{storeId}/{cartId}/{productId}/{branchId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> IncrementQuantity(int storeId, int cartId, int productId, int branchId)
        {
            var customerUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(customerUserAccountId);

            await _cartRepository.IncrementQuantity(customer.CustomerId, productId, storeId, cartId, branchId);

            return Ok(new { Success = 1, Message = "Incremented quantity successfully."});
        } 

        [HttpPut]
        [Route("/api/cart/decrement-quantity/{storeId}/{cartId}/{productId}/{branchId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DecrementQuantity(int storeId, int cartId, int productId, int branchId)
        {
            var customerUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(customerUserAccountId);

            await _cartRepository.DecrementQuantity(customer.CustomerId, productId, storeId, cartId, branchId);
            return Ok(new { Success = 1, Message = "Decremented quantity successfully." });
        }

    }
}
