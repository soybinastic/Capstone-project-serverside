using ConstructionMaterialOrderingApi.Dtos.SaleDtos;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IBestSellingReportRepository
    {
        Task AddReport(BestSellingReportDto bestSellingReportDto, int branchId);
        Task<List<BestSellingReport>> GetReportByBranchId(int branchId);
        Task<List<BestSellingReport>> GetReportsByWarehouseId(int warehouseId);
    }
}