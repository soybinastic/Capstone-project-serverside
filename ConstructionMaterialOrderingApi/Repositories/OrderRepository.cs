using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.CartDtos;
using ConstructionMaterialOrderingApi.Dtos.OrderDtos;
using ConstructionMaterialOrderingApi.Dtos.ProductDtos;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Hubs;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHardwareStoreRepository _hardwareStoreRepository;
        private readonly IHubContext<HardwareStoreHub> _hubContext;
        private readonly IConfirmedOrderRepository _confirmedOrderRepository;
        private readonly INotificationRepository _notificationRepository;

        public OrderRepository(ApplicationDbContext context, IHardwareStoreRepository hardwareStoreRepository,
            IHubContext<HardwareStoreHub> hubContext, IConfirmedOrderRepository confirmedOrderRepository,
            INotificationRepository notificationRepository)
        {
            _context = context;
            _hardwareStoreRepository = hardwareStoreRepository;
            _confirmedOrderRepository = confirmedOrderRepository;
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }
        public async Task<bool> PostOrder(PostOrderDto model, int customerId)
        {
            var isHardwareStoreExist = await _context.HardwareStores.Where(h => h.Id == model.HardwareStoreId)
                .FirstOrDefaultAsync();
            var customer = await _context.Customers.Where(c => c.Id == customerId).FirstOrDefaultAsync();

            if (customer == null)
                return false;


            if(isHardwareStoreExist != null && customer.IsVerified)
            {
                //if(UpdateProductStock(model.Products))
                //{

                //}

                //return false;
                //var productsInCart = ConvertToProductInCart(model.Products);
                var order = new Order()
                {
                    HardwareStoreId = model.HardwareStoreId,
                    BranchId = model.BranchId,
                    CustomerId = customerId,
                    CustomerName = $"{model.FirstName}, {model.LastName}",
                    CustomerEmail = model.Email,
                    Total = GenerateTotalPayment(model.Products),
                    Deliver = model.Deliver,
                    IsCustomerOrderRecieved = false,
                    IsOrderCanceled = false,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.PENDING,
                    IsApproved = false,
                    ShippingFee = model.ShippingFee
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
                    Address = model.Address,
                    Latitude = model.Latitude,
                    Longtitude = model.Longtitude
                    //NBIImageFile = await UploadImage(model.Nbi, "NBIFiles"),
                    //BarangayClearanceImageFile = await UploadImage(model.BarangayClearance, "BarangayClearanceFiles"),
                    //GovernmentIdImageFile = await UploadImage(model.GovernmentId, "GovernmentIdFiles"),
                    //BankStatementImageFile = await UploadImage(model.BankStatement, "BankStatementFiles"),
                    //Age = model.Age,
                    //BirthDate = model.BirthDate
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
                var orderNotificationNumber = await _context.OrderNotificationNumbers.Where(n => n.HardwareStoreId == model.HardwareStoreId && n.BranchId == model.BranchId).FirstOrDefaultAsync();
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
        private List<GetProductToCartDto> ConvertToProductInCart(string json)
        {
            List<GetProductToCartDto> productsInCart = JsonSerializer.Deserialize<List<GetProductToCartDto>>(json);
            return productsInCart;
        }
        
        private async Task<string> UploadImage(IFormFile file, string directoryFolder)
        {
            try
            {
                
                var folderName = Path.Combine("Resources", directoryFolder);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                if(file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return dbPath;
                }

                return string.Empty;
            }catch(Exception ex)
            {
                return "";
            }
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
                total += ((double)product.ProductQuantity * product.ProductPrice);
            });

            return total;
        }
        private async Task<string> GetHardwareStoreName(int hardwareStoreId) 
        {
            var hardwareStore = await _hardwareStoreRepository.GetHardwareByStoreId(hardwareStoreId);
            return hardwareStore.HardwareStoreName;
        }

        public async Task<List<GetOrderDto>> GetAllOrders(int hardwareStoreId, int branchId)
        {
            var listOfOrders = new List<GetOrderDto>(); 
            if(hardwareStoreId != 0)
            {
                var orders = await _context.Orders.Where(o => o.HardwareStoreId == hardwareStoreId && o.BranchId == branchId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync(); 
                foreach(var order in orders)
                {
                    var orderDto = new GetOrderDto()
                    {
                        OrderId = order.Id,
                        BranchId = order.BranchId,
                        HardwareStoreId = order.HardwareStoreId,
                        CustomerId = order.CustomerId,
                        CustomerName = order.CustomerName,
                        CustomerEmail = order.CustomerEmail,
                        OrderDate = order.OrderDate,
                        Total = order.Total,
                        Deliver = order.Deliver,
                        IsCustomerOrderRecieved = order.IsCustomerOrderRecieved,
                        IsOrderCanceled = order.IsOrderCanceled,
                        Status = order.Status,
                        IsApproved = order.IsApproved
                    };

                    listOfOrders.Add(orderDto);
                }
                var hardwareStoreOrderNotifNumber = await _context.OrderNotificationNumbers
                    .Where(n => n.HardwareStoreId == hardwareStoreId && n.BranchId == branchId)
                    .FirstOrDefaultAsync();
                hardwareStoreOrderNotifNumber.NumberOfOrder = 0;
                await _context.SaveChangesAsync();
                return listOfOrders;
            }

            return listOfOrders;
        }
        public async Task<Order> GetOrder(int orderId, int branchId)
        {
            var order = await _context.Orders.Where(o => o.Id == orderId && o.BranchId == branchId)
                .FirstOrDefaultAsync();
            return order;
        }

        public async Task<List<GetCustomerOrderHistoryDto>> GetAllCustomerOrdersHistory(int customerId)
        {
            var listOfCustomerOrderHistory = new List<GetCustomerOrderHistoryDto>();
            // if(customerId != 0)
            // {
            //     var customerOrderHistories = await _context.CustomerOrderHistories.Where(c => c.CustomerId == customerId)
            //         .ToListAsync(); 
            //     foreach(var customerOrderHistory in customerOrderHistories)
            //     {
            //         var customerOrderHistoryDto = new GetCustomerOrderHistoryDto()
            //         {
            //             CustomerOrderHistoryId = customerOrderHistory.Id,
            //             CustomerId = customerOrderHistory.CustomerId,
            //             OrderId = customerOrderHistory.OrderId,
            //             HardwareStoreId = customerOrderHistory.HardwareStoreId,
            //             HardwareStoreName = customerOrderHistory.HardwareStoreName,
            //             OrderDate = customerOrderHistory.OrderDate,
            //             Total = customerOrderHistory.Total,
            //             Deliver = customerOrderHistory.Deliver,
            //             IsRecieved = customerOrderHistory.IsRecieved
            //         };
            //         listOfCustomerOrderHistory.Add(customerOrderHistoryDto);
            //     }

            //     return listOfCustomerOrderHistory;
            // }
            var orders = await _context.Orders.Where(o => o.CustomerId == customerId)
                .ToListAsync();
            orders.ForEach((order) => 
                {
                    var branch = _context.Branches.FirstOrDefault(b => b.Id == order.BranchId);
                    listOfCustomerOrderHistory.Add(new GetCustomerOrderHistoryDto 
                        {
                            CustomerId = order.CustomerId,
                            OrderId = order.Id,
                            HardwareStoreId = order.HardwareStoreId,
                            BranchName = branch.Name,
                            BrancId = branch.Id,
                            OrderDate = order.OrderDate,
                            Total = order.Total,
                            Deliver = order.Deliver,
                            IsRecieved = order.IsCustomerOrderRecieved,
                            Status = order.Status,
                            ShippingFee = order.ShippingFee
                        });
                });
            return listOfCustomerOrderHistory.OrderByDescending(o => o.OrderDate).ToList();
        }

        public async Task<GetOrderNotificationNumber> GetOrderNotifNumber(int hardwareStoreId, int branchId)
        {
            var orderNotifNumber = await _context.OrderNotificationNumbers
                .Where(n => n.HardwareStoreId == hardwareStoreId && n.BranchId == branchId)
                .FirstOrDefaultAsync();

            var orderNotifNumberDto = new GetOrderNotificationNumber()
            {
                OrderNotifNumberId = orderNotifNumber.Id,
                HardwareStoreId = orderNotifNumber.HardwareStoreId,
                BranchId = orderNotifNumber.BranchId,
                NumberOfOrderNotif = orderNotifNumber.NumberOfOrder
            };

            return orderNotifNumberDto;
        }

        public async Task<GetCustomerOrderDetails> GetCustomerOrderDetails(int hardwareStoreId, int orderId, int branchId)
        {
            if(hardwareStoreId != 0 && orderId != 0)
            {
                var order = await _context.Orders.Where(o => o.Id == orderId && o.HardwareStoreId == hardwareStoreId && o.BranchId == branchId)
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
                    Email = customerOrderDetails.Email,
                    Longtitude = customerOrderDetails.Longtitude,
                    Latitude = customerOrderDetails.Latitude,
                    //Nbi = customerOrderDetails.NBIImageFile,
                    //BarangayClearance = customerOrderDetails.BarangayClearanceImageFile,
                    //GovernmentId = customerOrderDetails.GovernmentIdImageFile,
                    //BankStatement = customerOrderDetails.BankStatementImageFile,
                    //Age = customerOrderDetails.Age,
                    //BirthDate = customerOrderDetails.BirthDate
                };

                return customerOrderDetailDto;
            }

            return null;
        }

        public async Task<List<GetCustomerOrderProductDto>> GetCustomerOrderProducts(int hardwareStoreId, int orderId, int branchId)
        {
            var listOfOrderProducts = new List<GetCustomerOrderProductDto>();
            if(hardwareStoreId != 0 && orderId != 0)
            {
                var order = await _context.Orders.Where(o => o.HardwareStoreId == hardwareStoreId && o.Id == orderId && o.BranchId == branchId)
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

        public async Task<(bool,string)> UpdateOrder(int hardwareStoreId, int orderId, UpdateOrderDto model, int hardwareStoreUserId)
        {
            if(hardwareStoreId != 0 && orderId == model.OrderId)
            {
                DateTime datenow = DateTime.Now;

                var customerOrder = await _context.Orders.Where(o => o.HardwareStoreId == hardwareStoreId && o.CustomerId == model.CustomerId && o.Id == model.OrderId && o.BranchId == model.BranchId)
                    .FirstOrDefaultAsync();
                var orderProducts = await _context.CustomerOrderProducts.Where(op => op.OrderId == customerOrder.Id)
                    .ToListAsync();

                StringBuilder notification = new StringBuilder();
                //var customerOrderHistory = await _context.CustomerOrderHistories.Where(o => o.HardwareStoreId == hardwareStoreId && o.OrderId == model.OrderId)
                //.FirstOrDefaultAsync();
                if(customerOrder != null && orderProducts != null && customerOrder.IsApproved)
                {
                    //admin part
                    //customerOrder.CustomerName = model.CustomerName;
                    //customerOrder.CustomerEmail = model.CustomerEmail;
                    //customerOrder.IsCustomerOrderRecieved = model.IsCustomerOrderRecieved;

                    //customer hsitory part
                    //customerOrderHistory.IsRecieved = model.IsCustomerOrderRecieved; 
                    if (!model.IsCancelled)
                    {
                        //foreach (var orderProduct in orderProducts)
                        //{
                        //    var sale = await _context.Sales.Where(sale => sale.BranchId == model.BranchId && sale.HardwareProductId == orderProduct.ProductId
                        //        && sale.DateSale.Year == datenow.Year
                        //        && sale.DateSale.Month == datenow.Month
                        //        && sale.DateSale.Day == datenow.Day)
                        //        .FirstOrDefaultAsync();
                        //    if(sale != null)
                        //    {
                        //        sale.TotalSale += (orderProduct.ProductQuantity * orderProduct.ProductPrice);
                        //        await _context.SaveChangesAsync();
                        //    }
                        //    else
                        //    {
                        //        var newSale = new Sale()
                        //        {
                        //            BranchId = model.BranchId,
                        //            HardwareProductId = orderProduct.ProductId,
                        //            TotalSale = orderProduct.ProductQuantity * orderProduct.ProductPrice,
                        //            DateSale = DateTime.Now
                        //        };
                        //        await _context.Sales.AddAsync(newSale);
                        //        await _context.SaveChangesAsync();
                        //    }
                        //}

                        await _confirmedOrderRepository.Add(new ConfirmedOrder() 
                            {
                                BranchId = model.BranchId,
                                CustomerId = customerOrder.CustomerId,
                                OrderId = customerOrder.Id,
                                HardwareStoreUserId = hardwareStoreUserId,
                                DateConfirmed = DateTime.Now,
                                IsConfirmed = false
                            });

                        customerOrder.Status = "Completed";
                        customerOrder.IsOrderCanceled = false;
                        customerOrder.IsCustomerOrderRecieved = true;
                        customerOrder.DateConfirmed = DateTime.Now;

                        await _context.SaveChangesAsync();

                        var customer = await _context.Customers.Where(c => c.Id == customerOrder.CustomerId).FirstOrDefaultAsync();
                        notification.Append(customer.FirstName);
                        notification.Append(", ");
                        notification.Append(customer.LastName);
                        notification.Append("'s order on ");
                        notification.Append(customerOrder.OrderDate.ToString("dddd, dd MMMM yyyy tt"));
                        notification.Append(" is completed.");

                        await _notificationRepository.PushNotification(model.BranchId, notification.ToString(), "OrderStatus");
                        return (true, "Order completed");
                    }
                    else
                    {
                        await TurnBackProductQuantity(orderProducts, model.BranchId);

                        customerOrder.Status = "Cancelled";
                        customerOrder.IsOrderCanceled = true;
                        customerOrder.IsCustomerOrderRecieved = false;
                        customerOrder.DateConfirmed = DateTime.Now;

                        await _context.SaveChangesAsync();

                        var customer = await _context.Customers.Where(c => c.Id == customerOrder.CustomerId).FirstOrDefaultAsync();
                        notification.Append(customer.FirstName);
                        notification.Append(", ");
                        notification.Append(customer.LastName);
                        notification.Append("'s order on ");
                        notification.Append(customerOrder.OrderDate.ToString("dddd, dd MMMM yyyy tt"));
                        notification.Append(" is cancelled.");

                        await _notificationRepository.PushNotification(model.BranchId, notification.ToString(), "OrderStatus");
                        return (true, "Order cancelled");
                    }

                    //await _context.SaveChangesAsync();
                    //put real-time web functionality (temporary)
                    //await _hubContext.Clients.All.SendAsync("RecieveUpdateOrderToAdmin", customerOrder);
                    //await _hubContext.Clients.All.SendAsync("RecieveUpdateOrderToCustomer", customerOrderHistory);
                    //return (true,"");
                }

                return (false,"Order not found or no have approval");

            }

            return (false,"Something went wrong.");
        }

        public async Task<bool> ApproveOrder(int branchId, int orderId, int salesClerkId)
        {
            var orderToApprove = await _context.Orders.Where(o => o.BranchId == branchId && o.Id == orderId)
                .FirstOrDefaultAsync();
            var salesClerk = await _context.SalesClerks.FirstOrDefaultAsync(sc => sc.Id == salesClerkId);

            if(salesClerk == null) return false;

            if(orderToApprove != null && !orderToApprove.IsApproved)
            {
                orderToApprove.IsApproved = true;
                orderToApprove.Status = OrderStatus.PREPARING;

                await _context.OrderToPrepares.AddAsync(new OrderToPrepare
                    {
                        OrderId = orderToApprove.Id,
                        Order = orderToApprove,
                        SalesClerkId = salesClerk.Id,
                        SalesClerk = salesClerk
                    });

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        private async Task TurnBackProductQuantity(List<CustomerOrderProduct> orderProducts, int branchId)
        {
            foreach (var orderProduct in orderProducts)
            {
                var product = await _context.Products.Where(p => p.HardwareProductId == orderProduct.ProductId && p.BranchId == branchId)
                    .FirstOrDefaultAsync();
                if(product != null)
                {
                    product.StockNumber += orderProduct.ProductQuantity;
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
