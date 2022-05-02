using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Models;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IStoreAdminRepository
    {
        Task AddStoreAdmin(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreUserId);
        Task<StoreAdmin> GetStoreAdminByAccountId(string applicationUserId);
    }
}