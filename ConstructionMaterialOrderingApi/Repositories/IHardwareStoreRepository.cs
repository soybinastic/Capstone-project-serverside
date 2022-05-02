using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.AdminDtos;
using ConstructionMaterialOrderingApi.Dtos.HardwareStoreDtos;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IHardwareStoreRepository
    {
        HardwareStore AddHardwareStore(string accountId, RegisterHardwareStoreDto model);
        Task<GetHardwareStoreDto> GetHardware(string accountId);
        Task<GetHardwareStoreDto> GetHardwareByStoreId(int hardwareStoreId);
        Task<List<GetAllHardwareStoreDto>> GetAllHardwareStore();
        Task SignIn(string accountId, IList<string> role);
        Task SignOut(string accountId, IList<string> role);
    }
}
