using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public class WarehouseAdminCreator : IHardwareStoreUser
    {
        private readonly IWareHouseAdminRepository _warehouseAdminRepository;

        public WarehouseAdminCreator(IWareHouseAdminRepository warehouseAdminRepository)
        {
            _warehouseAdminRepository = warehouseAdminRepository;
        }
        public void CreateUser(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId)
        {
            _warehouseAdminRepository.AddWareHouseAdmin(storeUserDto,applicationUserId,hardwareStoreId);
        }

        public async Task<UserInformationDto> GetUser(string accountId)
        {
            var warehouseAdmin = await _warehouseAdminRepository
                .GetWarehouseAdminByAccountId(accountId);
            var userInformation = new UserInformationDto()
            {
                Id = warehouseAdmin.Id,
                AccountId = warehouseAdmin.AccountId,
                HardwareStoreId = warehouseAdmin.HardwareStoreId,
                WarehouseId = warehouseAdmin.WarehouseId,
                Name = warehouseAdmin.Name,
                Role = "WarehouseAdmin"
            };
            return userInformation;
        }
        
    }
}
