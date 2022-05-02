using ConstructionMaterialOrderingApi.Dtos.WarehouseProductStatusReportDto;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IProductStatusReportRepository
    {
        Task Add(ProductStatusReportDto statusReportDto, int warehouseReportId);
        Task<List<WarehouseProductStatusReport>> GetWarehouseProductStatusReports(int warehouseReportId);
        Task Remove(int warehouseProductStatusReportId);
    }
}