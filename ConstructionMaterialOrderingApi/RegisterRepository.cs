using ConstructionMaterialOrderingApi.Implementations;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi
{
    public static class RegisterRepository
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddTransient<IHardwareStoreRepository, HardwareStoreRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<ICartRepository, CartRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<ITransportAgentRepository, TransportAgentRepository>();
            services.AddTransient<IHardwareStoreUserRepository, HardwareStoreUserRepository>();
            services.AddTransient<IWareHouseAdminRepository, WareHouseAdminRepository>();
            services.AddTransient<ISuperAdminRepository, SuperAdminRepository>();
            services.AddTransient<IStoreAdminRepository, StoreAdminRepository>();
            services.AddTransient<IHardwareStoreUserHandler, HardwareStoreUserHandler>();
            services.AddTransient<IWarehouseRepository, WarehouseRepository>();
            services.AddTransient<IBranchRepository, BranchRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<IWarehouseProductRepository, WarehouseProductRepository>();
            services.AddTransient<IHardwareProductRepository, HardwareProductRepository>();
            services.AddTransient<IWarehouseReportRepository, WarehouseReportRepository>();
            services.AddTransient<IProductStatusReportRepository, ProductStatusReportRepository>();
            services.AddTransient<IProductStatusReport, ProductStatusReport>();
            services.AddTransient<IRecieveProductRepository, RecieveProductRepository>();
            services.AddTransient<IRecieveProductReport, RecieveProductReport>();
            services.AddTransient<IMoveProductRepository, MoveProductRepository>();
            services.AddTransient<IDeliverProductReportRepository, DeliverProductReportRepository>();
            services.AddTransient<IBranchProductFactory, BranchProductFactory>();
            services.AddTransient<IRequestRepository, RequestRepository>();
            services.AddTransient<IRequestProductRepository, RequestProductRepository>();
            services.AddTransient<ISaleRepository, SaleRepository>();
            services.AddTransient<IBestSellingReportRepository, BestSellingReportRepository>();
            services.AddTransient<IConfirmedOrderRepository, ConfirmedOrderRepository>();
            services.AddTransient<IDepositSlipRepository, DepositSlipRepository>();
            services.AddTransient<ISaleItemRepository, SaleItemRepository>();
            services.AddTransient<IRecipientRepository, RecipientRepository>();
            services.AddTransient<ICompanyRegisterRepository, CompanyRegisterRepository>();
            services.AddTransient<IVerificationRepository, VerificationRepository>();
        }
    }
}
