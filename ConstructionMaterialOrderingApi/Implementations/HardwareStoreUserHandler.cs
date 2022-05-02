using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
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

        public HardwareStoreUserHandler(ISuperAdminRepository superAdminRepository, ITransportAgentRepository transportAgentRepository,
            IWareHouseAdminRepository wareHouseAdminRepository, IStoreAdminRepository storeAdminRepository)
        {
            _superAdminRepository = superAdminRepository;
            _transportAgentRepository = transportAgentRepository;
            _wareHouseAdminRepository = wareHouseAdminRepository;
            _storeAdminRepository = storeAdminRepository;
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
