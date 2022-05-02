using ConstructionMaterialOrderingApi.Dtos.RequestDtos;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IRequestRepository
    {
        Task<List<Request>> GetAllRequestsByBranchId(int branchId);
        Task<List<Request>> GetAllRequestsByWarehouseId(int warehouseId);
        Task<(bool, string)> Send(RequestProductDto requestProductDto, int branchId);
    }
}