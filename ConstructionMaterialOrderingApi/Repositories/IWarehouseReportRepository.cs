using ConstructionMaterialOrderingApi.Dtos.WarehouseReportDtos;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IWarehouseReportRepository
    {
        Task<WarehouseReport> Add(WarehouseReportDto warehouseReportDto, int hardwareStoreId, int warehouseId);
        Task<WarehouseReport> GetWarehouseReport(int warehouseReportId, string reportType);
        Task RemoveReport(int warehouseReportId);
        Task<List<WarehouseReport>> GetWarehouseReports(int hardwareStoreId, string reportType);
        Task<List<WarehouseReport>> GetWarehouseReportsWithWarehouseId(int hardwareStoreId, string reportType, int warehouseId);
    }
}