using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IHardwareStoreUserRepository
    {
        Task AddUser(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId);
        Task<HardwareStoreUser> GetUserByAccountId(string applicationUserId);
        Task<HardwareStoreUser> GetUserByHardwareStoreId(int hardwareStoreId);
        Task<HardwareStoreUser> GetUserById(int id);
        Task<List<HardwareStoreUser>> GetUsers(int hardwareStoreId);
    }
}