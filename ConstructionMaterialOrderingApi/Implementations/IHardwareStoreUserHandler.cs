using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public interface IHardwareStoreUserHandler
    {
        IHardwareStoreUser GetHardwareStoreUserInstance(string role);
        void CreateUser(IHardwareStoreUser hardwareStoreUser, HardwareStoreUserDto storeUserDto, 
            string applicationUserId, int hardwareStoreId);
        Task<UserInformationDto> GetUser(IHardwareStoreUser hardwareStoreUser, string accountId);
    }
}
