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
        private readonly IStoreAdminRepository _storeAdminRepository;
        private readonly IHardwareStoreUserRepository _hardwareStoreUserRepository;
        private readonly IConfirmedOrderRepository _confirmedOrderRepository;

        public OrderController(ICustomerRepository customerRepository, IOrderRepository orderRepository, 
            IHardwareStoreRepository hardwareStoreRepository, UserManager<ApplicationUser> userManager,
            ITransportAgentRepository transportAgentRepository, IStoreAdminRepository storeAdminRepository, 
            IHardwareStoreUserRepository hardwareStoreUserRepository, 
            IConfirmedOrderRepository confirmedOrderRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _hardwareStoreRepository = hardwareStoreRepository;
            _userManager = userManager;
            _transportAgentRepository = transportAgentRepository;
            _storeAdminRepository = storeAdminRepository;
            _hardwareStoreUserRepository = hardwareStoreUserRepository;
            _confirmedOrderRepository = confirmedOrderRepository;
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

        [HttpGet("customer-order-products/{storeId}/{branchId}/{orderId}")] 
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerOrderProductsHistory([FromRoute]int storeId, [FromRoute]int branchId, [FromRoute]int orderId)
        {
            var orderProducts = await _orderRepository.GetCustomerOrderProducts(storeId, orderId, branchId);
            return Ok(ConvertToJsonString(orderProducts));
        }

        [HttpGet]
        [Route("/api/order/get-orders")]
        [Authorize(Roles = "StoreAdmin,TransportAgent")]
        public async Task<IActionResult> GetAllOrders()
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserAccountId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser); 

            if(role.FirstOrDefault() == "StoreAdmin")
            {
                var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(hardwareStoreUserAccountId);
                var orders = await _orderRepository.GetAllOrders(storeAdmin.HardwareStoreId, storeAdmin.BranchId);
                return Ok(ConvertOrdersToJsonObject(orders));
            }
            else if(role.FirstOrDefault() == "TransportAgent")
            {
                var transportAgent = await _transportAgentRepository.GetTransportAgentByAccountID(hardwareStoreUserAccountId);
                var orders = await _orderRepository.GetAllOrders(transportAgent.HardwareStoreId, transportAgent.BranchId);
                return Ok(ConvertOrdersToJsonObject(orders));
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong"});
        }  
        [HttpGet]
        [Route("/api/order/get-order-notif-number")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetOrderNotifNumber()
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(hardwareStoreUserAccountId);
            var notifNumber = await _orderRepository.GetOrderNotifNumber(storeAdmin.HardwareStoreId, storeAdmin.BranchId);
            var orderNotifNumberJson = JsonConvert.SerializeObject(notifNumber, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return Ok(orderNotifNumberJson);
        }
         
        [HttpGet]
        [Route("/api/order/get-customer-details/{orderId}")]
        [Authorize(Roles = "StoreAdmin,TransportAgent")]
        public async Task<IActionResult> GetCustumerOrderDetails(int orderId)
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserAccountId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser);
            if (role.FirstOrDefault() == "StoreAdmin")
            {
                var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(hardwareStoreUserAccountId);
                var customerOrderDetails = await _orderRepository.GetCustomerOrderDetails(storeAdmin.HardwareStoreId, orderId, storeAdmin.BranchId);
                return Ok(customerOrderDetails);
            }
            else if(role.FirstOrDefault() == "TransportAgent")
            {
                var hardwareStoreTranspAgent = await _transportAgentRepository.GetTransportAgentByAccountID(hardwareStoreUserAccountId);
                var customerOrderDetails = await _orderRepository.GetCustomerOrderDetails(hardwareStoreTranspAgent.HardwareStoreId, orderId, hardwareStoreTranspAgent.BranchId);
                return Ok(customerOrderDetails);
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong." });
        }

        [HttpGet]
        [Route("/api/order/get-order/{orderId}")]
        [Authorize(Roles = "StoreAdmin,TransportAgent")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            Order order = null;
            var appUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(appUserId);
            var roles = await _userManager.GetRolesAsync(user);
            if(roles.FirstOrDefault() == "StoreAdmin")
            {
                var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(appUserId);
                order = await _orderRepository.GetOrder(orderId, storeAdmin.BranchId);
            }
            else if(roles.FirstOrDefault() == "TransportAgent")
            {
                var transportAgent = await _transportAgentRepository.GetTransportAgentByAccountID(appUserId);
                order = await _orderRepository.GetOrder(orderId, transportAgent.BranchId);
            }

            var orderJson = JsonConvert.SerializeObject(order, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return Ok(orderJson);
        }

        [HttpGet]
        [Route("/api/order/get-order-products/{orderId}")]
        [Authorize(Roles = "StoreAdmin,TransportAgent")]
        public async Task<IActionResult> GetCustomerOrderProducts(int orderId)
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserAccountId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser);
            if (role.FirstOrDefault() == "StoreAdmin")
            {
                var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(hardwareStoreUserAccountId);
                var customerOrderProducts = await _orderRepository.GetCustomerOrderProducts(storeAdmin.HardwareStoreId,orderId, storeAdmin.BranchId);
                return Ok(ConvertCustomerOrderProductsToJsonObject(customerOrderProducts));
            }
            else if(role.FirstOrDefault() == "TransportAgent")
            {
                var transportAgent = await _transportAgentRepository.GetTransportAgentByAccountID(hardwareStoreUserAccountId);
                var customerOrderProducts = await _orderRepository.GetCustomerOrderProducts(transportAgent.HardwareStoreId, orderId, transportAgent.BranchId);
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
        [Authorize(Roles = "StoreAdmin,TransportAgent")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] UpdateOrderDto model)
        {
            var hardwareStoreUserAccountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hardwareStoreUser = await _userManager.FindByIdAsync(hardwareStoreUserAccountId);
            var role = await _userManager.GetRolesAsync(hardwareStoreUser);

            var user = await _hardwareStoreUserRepository.GetUserByAccountId(hardwareStoreUserAccountId);
            if(role.FirstOrDefault() == "StoreAdmin")
            {
                var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(hardwareStoreUserAccountId);
                (bool result, string message) = await _orderRepository.UpdateOrder(storeAdmin.HardwareStoreId, orderId, model, user.Id);
                return result ? Ok(new { Success = 1, Message = message}) : BadRequest(new { Success = 0, Message = message });
            } 
            else if(role.FirstOrDefault() == "TransportAgent")
            {
                var transportAgent = await _transportAgentRepository.GetTransportAgentByAccountID(hardwareStoreUserAccountId);
                (bool result, string message) = await _orderRepository.UpdateOrder(transportAgent.HardwareStoreId, orderId, model, user.Id);
                return result ? Ok(new { Success = 1, Message = message }) : BadRequest(new { Success = 0, Message = message });
            }

            return BadRequest(new { Success = 0, Message = "Something went wrong." });
        }

        [HttpGet]
        [Route("get-completed-orders")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetCompletedOrders()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var confirmedOrders = await _confirmedOrderRepository.GetConfirmedOrders(storeAdmin.BranchId);

            var jsonObject = JsonConvert.SerializeObject(confirmedOrders, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return Ok(jsonObject);
        }
        [HttpGet]
        [Route("get-completed-order/{orderId}")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetCompletedOrder([FromRoute]int orderId)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var confirmedOrder = await _confirmedOrderRepository.GetConfirmedOrder(storeAdmin.BranchId, orderId);

            var jsonObject = JsonConvert.SerializeObject(confirmedOrder, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return Ok(jsonObject);
        }

        [HttpPut]
        [Route("confirm-order/{orderId}")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> ConfirmOrder([FromRoute]int orderId)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);

            var result = await _confirmedOrderRepository.ConfirmOrder(orderId, storeAdmin.BranchId);
            return result ? Ok(new { Success = 1, Message = "Confirmed Successfully"}) : BadRequest(new { Success = 0, Message = "Failed to confirm"});
        }

        [HttpPut]
        [Route("approve-order/{orderId}")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> ApproveOrder([FromRoute]int orderId)
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var result = await _orderRepository.ApproveOrder(storeAdmin.BranchId, orderId);
            return result ? Ok(new { Success = 1, Message = "Order Approved"}) : BadRequest(new { Success = 0, Message = "Failed to approve"});
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

        private string ConvertToJsonString<T>(T obj)
        {
            var jsonObject = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
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
