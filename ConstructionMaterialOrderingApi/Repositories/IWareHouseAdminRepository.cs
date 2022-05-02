using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Models;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IWareHouseAdminRepository
    {
        Task AddWareHouseAdmin(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId);
        Task<WarehouseAdmin> GetWarehouseAdminByAccountId(string accountId);
        Task<WarehouseAdmin> GetWareHouseAdminByHardwareStoreId(int storeId);
    }
}