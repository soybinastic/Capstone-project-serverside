using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public interface IHardwareStoreUser
    {
        void CreateUser(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId);
        Task<UserInformationDto> GetUser(string accountId);
    }
}
