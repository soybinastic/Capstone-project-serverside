using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.OrderDtos;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IHardwareStoreRepository _hardwareStoreRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITransportAgentRepository _transportAgentRepository;

        public OrderController(ICustomerRepository customerRepository, IOrderRepository orderRepository, 
            IHardwareStoreRepository hardwareStoreRepository, UserManager<ApplicationUser> userManager,
            ITransportAgentRepository transportAgentRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _hardwareStoreRepository = hardwareStoreRepository;
            _userManager = userManager;
            _transportAgentRepository = transportAgentRepository;
        } 

        [HttpPost]
        [Route("/api/order/post-order")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PostOrder([FromBody] PostOrderDto model)
        {
            var customerUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(customerUserAccountId);

            var result = await _orderRepository.PostOrder(model, customer.CustomerId);

            return result ? Ok(new { Success = 1, Message = "Your order has been sent successfully."}) : BadRequest(new { Success = 0, Message = "Failed to post order."});
        } 

        [HttpGet]
        [Route("/api/order/get-customer-order-history")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerOrderHistories()
        {
            var customerUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(customerUserAccountId);

            var customerOrderHistories = await _orderRepository.GetAllCustomerOrdersHistory(customer.CustomerId);
            var customerOrderHistoriesJsonObject = JsonConvert.SerializeObject(customerOrderHistories, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return Ok(customerOrderHistoriesJsonObject);
        } 

        [HttpGet]
        [Route("/api/order/get-orders")]
        [Authorize(Roles = "StoreOwner,TransportAgent")]
        public async Task<IActionResult> GetAllOrders()
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserAccountId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser); 

            if(role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _hardwareStoreRepository.GetHardware(hardwareStoreUser.Id);
                var orders = await _orderRepository.GetAllOrders(hardwareStore.HardwareStoreId);
                return Ok(ConvertOrdersToJsonObject(orders));
            }
            else if(role.FirstOrDefault() == "TransportAgent")
            {
                var hardwareStore = await _transportAgentRepository.GetTransportAgentByAccountId(hardwareStoreUserAccountId);
                var orders = await _orderRepository.GetAllOrders(hardwareStore.HardwareStoreId);
                return Ok(ConvertOrdersToJsonObject(orders));
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong"});
        }  
        [HttpGet]
        [Route("/api/order/get-order-notif-number")]
        [Authorize(Roles = "StoreOwner")]
        public async Task<IActionResult> GetOrderNotifNumber()
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserAccountId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser); 
            if(role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _hardwareStoreRepository.GetHardware(hardwareStoreUser.Id);
                var orderNotifNumber = await _orderRepository.GetOrderNotifNumber(hardwareStore.HardwareStoreId);
                return Ok(orderNotifNumber);
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong."});
        }
         
        [HttpGet]
        [Route("/api/order/get-customer-details/{orderId}")]
        [Authorize(Roles = "StoreOwner,TransportAgent")]
        public async Task<IActionResult> GetCustumerOrderDetails(int orderId)
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserAccountId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser);
            if (role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _hardwareStoreRepository.GetHardware(hardwareStoreUser.Id);
                var customerOrderDetails = await _orderRepository.GetCustomerOrderDetails(hardwareStore.HardwareStoreId, orderId);
                return Ok(customerOrderDetails);
            }
            else if(role.FirstOrDefault() == "TransportAgent")
            {
                var hardwareStoreTranspAgent = await _transportAgentRepository.GetTransportAgentByAccountId(hardwareStoreUserAccountId);
                var customerOrderDetails = await _orderRepository.GetCustomerOrderDetails(hardwareStoreTranspAgent.HardwareStoreId, orderId);
                return Ok(customerOrderDetails);
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong." });
        }

        [HttpGet]
        [Route("/api/order/get-order-products/{orderId}")]
        [Authorize(Roles = "StoreOwner")]
        public async Task<IActionResult> GetCustomerOrderProducts(int orderId)
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserAccountId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser);
            if (role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _hardwareStoreRepository.GetHardware(hardwareStoreUser.Id);
                var customerOrderProducts = await _orderRepository.GetCustomerOrderProducts(hardwareStore.HardwareStoreId,orderId);
                return Ok(ConvertCustomerOrderProductsToJsonObject(customerOrderProducts));
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong." });
        }

        [HttpGet]
        [Route("/api/order/get-order-products-history/{orderId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAllCustomerOrderProductsHistory(int orderId)
        {
            var customerUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _customerRepository.GetCustomerByAccountId(customerUserAccountId);

            var customerOrderProductHistories = await _orderRepository.GetCustomerOrderProductsHistory(customer.CustomerId, orderId);
            var customerOrderProductHistoriesJsonObject = JsonConvert.SerializeObject(customerOrderProductHistories, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return Ok(customerOrderProductHistoriesJsonObject);
        } 

        [HttpPut]
        [Route("/api/order/update-order/{orderId}")]
        [Authorize(Roles = "StoreOwner,TransportAgent")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] UpdateOrderDto model)
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserAccountId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser);
            if(role.FirstOrDefault() == "StoreOwner")
            {
                var hardwareStore = await _hardwareStoreRepository.GetHardware(hardwareStoreUserAccountId);
                var result = await _orderRepository.UpdateOrder(hardwareStore.HardwareStoreId, orderId, model);
                return result ? Ok(new { Success = 1, Message = "Order has been updated successfully."}) : BadRequest(new { Success = 0, Message = "Failed to update" });
            } 
            else if(role.FirstOrDefault() == "TransportAgent")
            {
                var hardwareStoreTrasnpAgent = await _transportAgentRepository.GetTransportAgentByAccountId(hardwareStoreUserAccountId);
                var result = await _orderRepository.UpdateOrder(hardwareStoreTrasnpAgent.HardwareStoreId, orderId, model);
                return result ? Ok(new { Success = 1, Message = "Order has been updated successfully." }) : BadRequest(new { Success = 0, Message = "Failed to update" });
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong." });
        }

        private string ConvertCustomerOrderProductsToJsonObject(List<GetCustomerOrderProductDto> customerOrderProducts)
        {
            var jsonObject = JsonConvert.SerializeObject(customerOrderProducts, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return jsonObject;
        }

        private string ConvertOrdersToJsonObject(List<GetOrderDto> orders)
        {
            var ordersJsonObject = JsonConvert.SerializeObject(orders, new JsonSerializerSettings
            { 
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return ordersJsonObject;
        }
    }
}
