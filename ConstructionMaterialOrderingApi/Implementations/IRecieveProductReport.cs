using ConstructionMaterialOrderingApi.Dtos.RecieveProductDtos;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public interface IRecieveProductReport
    {
        Task<(bool, string)> Add(AddRecieveProductDto recieveProductDto, int hardwareStoreId, int warehouseId);
        Task<RecieveProduct> GetRecieveProduct(int recieveProductId);
        Task<List<RecieveProduct>> GetRecieveProducts(int warehouseReportId);
        Task<WarehouseReport> GetReciveProductReport(int warehouseReportId);
        Task<List<WarehouseReport>> GetReciveProductReports(int warehouseId, int hardwareStoreId);
        Task<bool> UpdateRecieveProduct(RecieveProductDto recieveProductDto, int recieveProductId, int warehouseId);
        Task<bool> AddRecieveProduct(RecieveProductDto recieveProductDto, int warehouseReportId, int warehouseId);
    }
}