using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.CartDtos;
using ConstructionMaterialOrderingApi.Dtos.OrderDtos;
using ConstructionMaterialOrderingApi.Dtos.ProductDtos;
using ConstructionMaterialOrderingApi.Hubs;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHardwareStoreRepository _hardwareStoreRepository;
        private readonly IHubContext<HardwareStoreHub> _hubContext;

        public OrderRepository(ApplicationDbContext context, IHardwareStoreRepository hardwareStoreRepository,
            IHubContext<HardwareStoreHub> hubContext)
        {
            _context = context;
            _hardwareStoreRepository = hardwareStoreRepository;
            _hubContext = hubContext;
        }
        public async Task<bool> PostOrder(PostOrderDto model, int customerId)
        {
            var isHardwareStoreExist = await _context.HardwareStores.Where(h => h.Id == model.HardwareStoreId)
                .FirstOrDefaultAsync(); 
            if(isHardwareStoreExist != null)
            {
                if(UpdateProductStock(model.Products))
                {
                    var order = new Order()
                    {
                        HardwareStoreId = model.HardwareStoreId,
                        CustomerId = customerId,
                        CustomerName = $"{model.FirstName}, {model.LastName}",
                        CustomerEmail = model.Email,
                        Total = GenerateTotalPayment(model.Products),
                        Deliver = model.Deliver,
                        IsCustomerOrderRecieved = false,
                        OrderDate = DateTime.Now
                    };

                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();

                    var customerOrderDatails = new CustomerOrderDetails()
                    {
                        OrderId = order.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        ContactNo = model.ContactNo,
                        Address = model.Address
                    };

                    var customerOrderHistory = new CustomerOrderHistory()
                    {
                        OrderId = order.Id,
                        CustomerId = customerId,
                        HardwareStoreId = model.HardwareStoreId,
                        HardwareStoreName = await GetHardwareStoreName(model.HardwareStoreId),
                        Total = order.Total,
                        Deliver = model.Deliver,
                        IsRecieved = false,
                        OrderDate = order.OrderDate
                    };

                    await _context.CustomerOrderDetails.AddAsync(customerOrderDatails);
                    await _context.CustomerOrderHistories.AddAsync(customerOrderHistory);
                    await _context.SaveChangesAsync();

                    foreach (var orderProduct in model.Products)
                    {
                        var customerOrderProduct = new CustomerOrderProduct()
                        {
                            OrderId = order.Id,
                            ProductId = orderProduct.ProductId,
                            CategoryId = orderProduct.CategoryId,
                            ProductName = orderProduct.ProductName,
                            ProductBrand = orderProduct.ProductBrand,
                            ProductQuality = orderProduct.ProductQuality,
                            ProductDescription = orderProduct.ProductDescription,
                            ProductPrice = orderProduct.ProductPrice,
                            ProductQuantity = orderProduct.ProductQuantity
                        };
                        var customerOrderProductHistory = new CustomerOrderProductHistory()
                        {
                            CustomerOrderHistoryId = customerOrderHistory.Id,
                            ProductId = orderProduct.ProductId,
                            CategoryId = orderProduct.CategoryId,
                            ProductName = orderProduct.ProductName,
                            ProductDescription = orderProduct.ProductDescription,
                            ProductBrand = orderProduct.ProductBrand,
                            ProductQuality = orderProduct.ProductQuality,
                            ProductPrice = orderProduct.ProductPrice,
                            ProductQuantity = orderProduct.ProductQuantity
                        };

                        await _context.CustomerOrderProducts.AddAsync(customerOrderProduct);
                        await _context.CustomerOrderProductHistories.AddAsync(customerOrderProductHistory);
                        await _context.SaveChangesAsync();
                    }
                    var orderNotificationNumber =  await _context.OrderNotificationNumbers.Where(n => n.HardwareStoreId == model.HardwareStoreId).FirstOrDefaultAsync();
                    orderNotificationNumber.NumberOfOrder += 1;
                    await _context.SaveChangesAsync();

                    DeleteAllProductsFromCart(model.Products);

                    //put temporary realtime web functionality
                    await _hubContext.Clients.All.SendAsync("RecieveOrder", order);
                    await _hubContext.Clients.All.SendAsync("RecieveOrderNotif", orderNotificationNumber);

                    return true;
                }

                return false;

            }
            return false;
            
        } 
        private void DeleteAllProductsFromCart(List<GetProductToCartDto> productFromCartDto)
        {
            foreach(var productDto in productFromCartDto)
            {
                var productInCart = _context.Carts.Where(c => c.Id == productDto.CartId).FirstOrDefault();
                _context.Carts.Remove(productInCart);
                _context.SaveChanges();
            }
        }
        private bool UpdateProductStock(List<GetProductToCartDto> productFromCartDto)
        {
            bool result = false;
            foreach(var productDto in productFromCartDto)
            {
                var product = _context.Products
                .Where(p => p.Id == productDto.ProductId && p.HardwareStoreId == productDto.HardwareStoreId)
                .FirstOrDefault();

                product.StockNumber -= productDto.ProductQuantity;

                if (product.StockNumber >= 0)
                {
                    result = true;
                    product.IsAvailable = IsAvailableProductYet(product.StockNumber);
                    _context.SaveChanges();
                }
                else
                {
                    result = false;
                    break;
                }
            }

            return result;
        } 
        private bool IsAvailableProductYet(int stockNumber)
        {
            if(stockNumber > 0)
            {
                return true;
            }

            return false;
        }
        private double GenerateTotalPayment(List<GetProductToCartDto> productFromCartDto)
        {
            double total = 0;
            productFromCartDto.ForEach((product) => 
            {
                total += (product.ProductQuantity * product.ProductPrice);
            });

            return total;
        }
        private async Task<string> GetHardwareStoreName(int hardwareStoreId) 
        {
            var hardwareStore = await _hardwareStoreRepository.GetHardwareByStoreId(hardwareStoreId);
            return hardwareStore.HardwareStoreName;
        }

        public async Task<List<GetOrderDto>> GetAllOrders(int hardwareStoreId)
        {
            var listOfOrders = new List<GetOrderDto>(); 
            if(hardwareStoreId != 0)
            {
                var orders = await _context.Orders.Where(o => o.HardwareStoreId == hardwareStoreId)
                    .ToListAsync(); 
                foreach(var order in orders)
                {
                    var orderDto = new GetOrderDto()
                    {
                        OrderId = order.Id,
                        HardwareStoreId = order.HardwareStoreId,
                        CustomerId = order.CustomerId,
                        CustomerName = order.CustomerName,
                        CustomerEmail = order.CustomerEmail,
                        OrderDate = order.OrderDate,
                        Total = order.Total,
                        Deliver = order.Deliver,
                        IsCustomerOrderRecieved = order.IsCustomerOrderRecieved
                    };

                    listOfOrders.Add(orderDto);
                }
                var hardwareStoreOrderNotifNumber = await _context.OrderNotificationNumbers
                    .Where(n => n.HardwareStoreId == hardwareStoreId)
                    .FirstOrDefaultAsync();
                hardwareStoreOrderNotifNumber.NumberOfOrder = 0;
                await _context.SaveChangesAsync();
                return listOfOrders;
            }

            return listOfOrders;
        }

        public async Task<List<GetCustomerOrderHistoryDto>> GetAllCustomerOrdersHistory(int customerId)
        {
            var listOfCustomerOrderHistory = new List<GetCustomerOrderHistoryDto>();
            if(customerId != 0)
            {
                var customerOrderHistories = await _context.CustomerOrderHistories.Where(c => c.CustomerId == customerId)
                    .ToListAsync(); 
                foreach(var customerOrderHistory in customerOrderHistories)
                {
                    var customerOrderHistoryDto = new GetCustomerOrderHistoryDto()
                    {
                        CustomerOrderHistoryId = customerOrderHistory.Id,
                        CustomerId = customerOrderHistory.CustomerId,
                        OrderId = customerOrderHistory.OrderId,
                        HardwareStoreId = customerOrderHistory.HardwareStoreId,
                        HardwareStoreName = customerOrderHistory.HardwareStoreName,
                        OrderDate = customerOrderHistory.OrderDate,
                        Total = customerOrderHistory.Total,
                        Deliver = customerOrderHistory.Deliver,
                        IsRecieved = customerOrderHistory.IsRecieved
                    };
                    listOfCustomerOrderHistory.Add(customerOrderHistoryDto);
                }

                return listOfCustomerOrderHistory;
            }

            return listOfCustomerOrderHistory;
        }

        public async Task<GetOrderNotificationNumber> GetOrderNotifNumber(int hardwareStoreId)
        {
            var orderNotifNumber = await _context.OrderNotificationNumbers
                .Where(n => n.HardwareStoreId == hardwareStoreId)
                .FirstOrDefaultAsync();

            var orderNotifNumberDto = new GetOrderNotificationNumber()
            {
                OrderNotifNumberId = orderNotifNumber.Id,
                HardwareStoreId = orderNotifNumber.HardwareStoreId,
                NumberOfOrderNotif = orderNotifNumber.NumberOfOrder
            };

            return orderNotifNumberDto;
        }

        public async Task<GetCustomerOrderDetails> GetCustomerOrderDetails(int hardwareStoreId, int orderId)
        {
            if(hardwareStoreId != 0 && orderId != 0)
            {
                var order = await _context.Orders.Where(o => o.Id == orderId && o.HardwareStoreId == hardwareStoreId)
                    .FirstOrDefaultAsync();

                var customerOrderDetails = await _context.CustomerOrderDetails.Where(d => d.OrderId == order.Id)
                    .FirstOrDefaultAsync();
                var customerOrderDetailDto = new GetCustomerOrderDetails()
                {
                    CustomerOrderDetailId = customerOrderDetails.Id,
                    OrderId = customerOrderDetails.OrderId,
                    FirstName = customerOrderDetails.FirstName,
                    LastName = customerOrderDetails.LastName,
                    Address = customerOrderDetails.Address,
                    ContactNo = customerOrderDetails.ContactNo,
                    Email = customerOrderDetails.Email
                };

                return customerOrderDetailDto;
            }

            return null;
        }

        public async Task<List<GetCustomerOrderProductDto>> GetCustomerOrderProducts(int hardwareStoreId, int orderId)
        {
            var listOfOrderProducts = new List<GetCustomerOrderProductDto>();
            if(hardwareStoreId != 0 && orderId != 0)
            {
                var order = await _context.Orders.Where(o => o.HardwareStoreId == hardwareStoreId && o.Id == orderId)
                    .FirstOrDefaultAsync(); 
                if(order != null)
                { 
                    
                    var customerOrderProducts = await _context.CustomerOrderProducts.Where(p => p.OrderId == order.Id)
                        .ToListAsync();
                    customerOrderProducts.ForEach((product) => 
                    {
                        var orderProductsDto = new GetCustomerOrderProductDto()
                        {
                            CustomerOrderProductId = product.Id,
                            OrderId = product.OrderId,
                            ProductId = product.ProductId,
                            CategoryId = product.CategoryId,
                            ProductName = product.ProductName,
                            ProductDescription = product.ProductDescription,
                            ProductBrand = product.ProductBrand,
                            ProductQuality = product.ProductQuality,
                            ProductQuantity = product.ProductQuantity,
                            ProductPrice = product.ProductPrice
                        };

                        listOfOrderProducts.Add(orderProductsDto);
                    });

                    return listOfOrderProducts;
                }
            }

            return listOfOrderProducts;
        }

        public async Task<List<GetCustomerOrderProductHistoryDto>> GetCustomerOrderProductsHistory(int customerId, int orderId)
        {
            var listOfOrderProductsHistory = new List<GetCustomerOrderProductHistoryDto>();
            if(customerId != 0 && orderId != 0)
            {
                var customerOrderHistory = await _context.CustomerOrderHistories.Where(c => c.OrderId == orderId).FirstOrDefaultAsync(); 
                if(customerOrderHistory != null)
                {
                    var orderProducts = await _context.CustomerOrderProductHistories
                        .Where(p => p.CustomerOrderHistoryId == customerOrderHistory.Id)
                        .ToListAsync();

                    orderProducts.ForEach((product) => 
                    {
                        var orderProductDto = new GetCustomerOrderProductHistoryDto()
                        {
                            CustomerOrderProductHistoryId = product.Id,
                            CustomerOrderHistoryId = product.CustomerOrderHistoryId,
                            CategoryId = product.CategoryId,
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            ProductBrand = product.ProductBrand,
                            ProductDescription = product.ProductDescription,
                            ProductQuality = product.ProductQuality,
                            ProductPrice = product.ProductPrice,
                            ProductQuantity = product.ProductQuantity
                        };

                        listOfOrderProductsHistory.Add(orderProductDto);
                    });

                    return listOfOrderProductsHistory;
                }
            }

            return listOfOrderProductsHistory;
        }

        public async Task<bool> UpdateOrder(int hardwareStoreId, int orderId, UpdateOrderDto model)
        {
            if(hardwareStoreId != 0 && orderId == model.OrderId)
            {
                var customerOrder = await _context.Orders.Where(o => o.HardwareStoreId == hardwareStoreId && o.Id == model.OrderId)
                    .FirstOrDefaultAsync();
                var customerOrderHistory = await _context.CustomerOrderHistories.Where(o => o.HardwareStoreId == hardwareStoreId && o.OrderId == model.OrderId)
                    .FirstOrDefaultAsync();
                if(customerOrder != null && customerOrderHistory != null)
                {
                    //admin part
                    //customerOrder.CustomerName = model.CustomerName;
                    //customerOrder.CustomerEmail = model.CustomerEmail;
                    customerOrder.IsCustomerOrderRecieved = model.IsCustomerOrderRecieved;

                    //customer hsitory part
                    customerOrderHistory.IsRecieved = model.IsCustomerOrderRecieved;

                    await _context.SaveChangesAsync();
                    //put real-time web functionality (temporary)
                    await _hubContext.Clients.All.SendAsync("RecieveUpdateOrderToAdmin", customerOrder);
                    await _hubContext.Clients.All.SendAsync("RecieveUpdateOrderToCustomer", customerOrderHistory);
                    return true;
                }

                return false;

            }

            return false;
        }
    }
}
