using ConstructionMaterialOrderingApi.Dtos.WarehouseProductStatusReportDto;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public interface IProductStatusReport
    {
        Task<(bool, string)> Add(AddProductStatusReportDto statusReportDto, int hardwareStoreId, int warehouseId);
        Task<List<WarehouseProductStatusReport>> GetProductStatusReports(int warehouseReportId);
        Task<List<WarehouseReport>> GetReports(int hardwareStoreId);
        Task<WarehouseReport> GetReport(int warehouseReportId);
        Task RemoveStatusReport(int statusReportId);
        Task RemoveReport(int warehouseReportId);
        Task AddProductStatusReport(int warehouseReportId, ProductStatusReportDto statusReportDto);
    }
}