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
    public class SuperAdmin : IHardwareStoreUser
    {
        private readonly ISuperAdminRepository _superAdminRepository;
        public SuperAdmin(ISuperAdminRepository superAdminRepository)
        {
            _superAdminRepository = superAdminRepository;
        }
        public void CreateUser(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId)
        {
            _superAdminRepository.AddSuperAdmin(storeUserDto, applicationUserId, hardwareStoreId);
        }

        public async Task<UserInformationDto> GetUser(string accountId)
        {
            var superAdmin = await _superAdminRepository
                .GetSuperAdminByAccountId(accountId);
            var userInformation = new UserInformationDto()
            {
                Id = superAdmin.Id,
                AccountId = superAdmin.AccountId,
                HardwareStoreId = superAdmin.HardwareStoreId,
                Name = superAdmin.Name,
                Role = "SuperAdmin"
            };
            return userInformation;
        }
        
    }
}
