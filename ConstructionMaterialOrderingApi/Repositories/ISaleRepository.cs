using ConstructionMaterialOrderingApi.Dtos.SaleDtos;
using ConstructionMaterialOrderingApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface ISaleRepository
    {
        Task<List<Sale>> GetBestSellingProduct(int branchId, DateTime dateTime, string filterBy);
        Task<double> GetTodaySales(int branchId);
        Task<SaleDto> GetTotalSales(int branchId, int year);
        Task<double> GetSales(int branchId, DateTime date, string filterBy);

        Task<List<SaleReportDetails>> GetSaleReports(int branchId);
        Task AddSaleReport(SaleReportDto saleReportDto, int branchId);
        Task<SummarySaleDto> GetMonthSalesSummary(int branchId, DateTime date);
    }
}