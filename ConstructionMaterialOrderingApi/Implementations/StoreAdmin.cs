using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public class StoreAdmin : IHardwareStoreUser
    {
        private readonly IStoreAdminRepository _storeAdminRepository;

        public StoreAdmin(IStoreAdminRepository storeAdminRepository)
        {
            _storeAdminRepository = storeAdminRepository;
        }
        public void CreateUser(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId)
        {
            _storeAdminRepository.AddStoreAdmin(storeUserDto, applicationUserId, hardwareStoreId);
        }

        public async Task<UserInformationDto> GetUser(string accountId)
        {
            var storeAdmin = await _storeAdminRepository
                .GetStoreAdminByAccountId(accountId);
            var userInformation = new UserInformationDto()
            {
                Id = storeAdmin.Id,
                AccountId = storeAdmin.AccountId,
                Name = storeAdmin.Name,
                Role = "StoreAdmin",
                HardwareStoreId = storeAdmin.HardwareStoreId,
                BranchId = storeAdmin.BranchId
            };
            return userInformation;
        }
        
    }
}
