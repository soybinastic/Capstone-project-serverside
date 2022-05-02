using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Models;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface ISuperAdminRepository
    {
        Task AddSuperAdmin(HardwareStoreUserDto storeDto, string applicationUserId, int hardwareStoreId);
        Task<SuperAdmin> GetSuperAdminByAccountId(string accountId);
        Task<SuperAdmin> GetSuperAdminByHardwareStoreId(int hardwareStoreId);
    }
}