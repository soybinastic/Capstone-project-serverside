using ConstructionMaterialOrderingApi.Dtos.BranchDto;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IBranchRepository
    {
        Task AddBranch(AddBranchDto branchDto, int hardwareStoreId);
        Task<Branch> GetBranch(int branchId);
        Task<List<Branch>> GetBranchesByStoreId(int hardwareStoreId);
        Task<List<Branch>> GetActiveBranches(int hardwareStoreId);
        Task<List<BranchDto>> GetAllBranches(double lat = 0, double lng = 0, double adjustedKm = 0);
        Task<List<BranchDto>> Search(string search, double lat = 0, double lng = 0, double adjustedKm = 0);
        Task<bool> UpdateBranch(UpdateBranchDto branchDto, int branchId, int hardwareStoreId);
    }
}