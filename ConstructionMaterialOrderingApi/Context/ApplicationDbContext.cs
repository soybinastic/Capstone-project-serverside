using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<HardwareStore> HardwareStores { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CustomerOrderProduct> CustomerOrderProducts { get; set; }
        public DbSet<CustomerOrderDetails> CustomerOrderDetails { get; set; }
        public DbSet<CustomerOrderHistory> CustomerOrderHistories { get; set; }
        public DbSet<CustomerOrderProductHistory> CustomerOrderProductHistories { get; set; }
        public DbSet<OrderNotificationNumber> OrderNotificationNumbers { get; set; }
        public DbSet<TransportAgent> TransportAgents { get; set; }
        public DbSet<HardwareStoreUser> HardwareStoreUsers { get; set; }
        public DbSet<SuperAdmin> SuperAdmins { get; set; }
        public DbSet<WarehouseAdmin> WarehouseAdmins { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<StoreAdmin> StoreAdmins { get; set; }
        public DbSet<HardwareProduct> HardwareProducts { get; set; }
        public DbSet<RecieveProduct> RecieveProducts { get; set; }
        public DbSet<MoveProduct> MoveProducts { get; set; }
        public DbSet<WarehouseProduct> WarehouseProducts { get; set; }
        public DbSet<BranchNotification> BranchNotifications { get; set; }
        public DbSet<NotificationNumber> NotificationNumbers { get; set; }
        public DbSet<WarehouseNotification> WarehouseNotifications { get; set; }
        public DbSet<WarehouseNotificationNumber> WarehouseNotificationNumbers { get; set; }
        public DbSet<WarehouseReport> WarehouseReports { get; set; }
        public DbSet<WarehouseProductStatusReport> WarehouseProductStatusReports { get; set; }
        public DbSet<DeliverProduct> DeliverProducts { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestProduct> RequestProducts { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<BestSellingReport> BestSellingReports { get; set; }
        public DbSet<BestSellingDetail> BestSellingDetails { get; set; }
        public DbSet<BestSellingProductReport> BestSellingProductReports { get; set; }
        public DbSet<SaleReportDetails> SaleReportDetails { get; set; }
        public DbSet<CustomerOrderRecieveImage> CustomerOrderRecieveImages { get; set; }
        public DbSet<ConfirmedOrder> ConfirmedOrders { get; set; }
        public DbSet<DepositSlip> DepositSlips { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<SaleItemSummary> SaleItemSummaries { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<RecipientItem> RecipientItems { get; set; }
        public DbSet<CompanyRegister> CompanyRegisters { get; set; }
        public DbSet<CustomerVerification> CustomerVerifications { get; set; }
        public DbSet<Dashboard> Dashboard { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<Cashier> Cashiers { get; set; }
        public DbSet<SalesClerk> SalesClerks { get; set; }
        public DbSet<OrderToPrepare> OrderToPrepares { get; set; }
        public DbSet<FastlineUser> FastlineUsers { get; set; }
        public DbSet<VerifiedUser> VerifiedUsers { get; set; }
        public DbSet<RegisteredCompany> RegisteredCompanies { get; set; }
    }
}
