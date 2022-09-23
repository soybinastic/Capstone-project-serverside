using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Models;
using ConstructionMaterialOrderingApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public class HardwareStoreUserHandler : IHardwareStoreUserHandler
    {
        private readonly ISuperAdminRepository _superAdminRepository;
        private readonly ITransportAgentRepository _transportAgentRepository;
        private readonly IWareHouseAdminRepository _wareHouseAdminRepository;
        private readonly IStoreAdminRepository _storeAdminRepository;
        private readonly IBranchUserRepository<Cashier> _cashierRepository;
        private readonly IBranchUserRepository<SalesClerk> _salesClerkRepository;

        public HardwareStoreUserHandler(ISuperAdminRepository superAdminRepository, ITransportAgentRepository transportAgentRepository,
            IWareHouseAdminRepository wareHouseAdminRepository, IStoreAdminRepository storeAdminRepository,
            IBranchUserRepository<Cashier> cashierRepository, IBranchUserRepository<SalesClerk> salesClerkRepository)
        {
            _superAdminRepository = superAdminRepository;
            _transportAgentRepository = transportAgentRepository;
            _wareHouseAdminRepository = wareHouseAdminRepository;
            _storeAdminRepository = storeAdminRepository;
            _cashierRepository = cashierRepository;
            _salesClerkRepository = salesClerkRepository;
        }

        public void CreateUser(IHardwareStoreUser hardwareStoreUser, HardwareStoreUserDto storeUserDto,
            string applicationUserId, int hardwareStoreId)
        {
            hardwareStoreUser.CreateUser(storeUserDto, applicationUserId, hardwareStoreId);
        }

        public IHardwareStoreUser GetHardwareStoreUserInstance(string role)
        {
            switch(role)
            {
                case "SuperAdmin":
                    return new SuperAdmin(_superAdminRepository);
                case "TransportAgent":
                    return new TransportAgent(_transportAgentRepository);
                case "WarehouseAdmin":
                    return new WarehouseAdminCreator(_wareHouseAdminRepository);
                case "StoreAdmin":
                    return new StoreAdmin(_storeAdminRepository);
                case UserRole.CASHIER:
                    return new CashierCreator(_cashierRepository);
                case UserRole.SALES_CLERK:
                    return new SalesClerkCreator(_salesClerkRepository);
                default:
                    return null;
            };
        }

        public async Task<UserInformationDto> GetUser(IHardwareStoreUser hardwareStoreUser, string accountId)
        {
            var user = await hardwareStoreUser.GetUser(accountId);
            return user;
        }
    }
}
