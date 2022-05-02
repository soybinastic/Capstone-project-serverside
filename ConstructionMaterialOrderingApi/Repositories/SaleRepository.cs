using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.SaleDtos;
using Microsoft.EntityFrameworkCore;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly ApplicationDbContext _context;
        public SaleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SummarySaleDto> GetMonthSalesSummary(int branchId, DateTime date)
        {
            var dailySales = new List<TotalSaleDto>();
            var listOfDailySales = await _context.Sales.Where(sale => sale.BranchId == branchId
                && sale.DateSale.Year == date.Year
                && sale.DateSale.Month == date.Month)
                .OrderBy(sale => sale.DateSale.Day)
                .ToListAsync();

            foreach (var sale in listOfDailySales)
            {
                var isExist = dailySales.Any(s => s?.DateSale.Year == sale.DateSale.Year && s?.DateSale.Month == sale.DateSale.Month && s?.DateSale.Day == sale.DateSale.Day);
                if (!isExist)
                {
                    double total = listOfDailySales.Where(s => s.DateSale.Year == sale.DateSale.Year && s.DateSale.Month == sale.DateSale.Month && s.DateSale.Day == sale.DateSale.Day)
                        .Sum(s => s.TotalSale);
                    dailySales.Add(new TotalSaleDto { DateSale = sale.DateSale, TotalSale = (decimal)total});
                }
            }

            double totalMonthSales = listOfDailySales.Sum(dailySale => dailySale.TotalSale);

            var summarySales = new SummarySaleDto(dailySales, date, totalMonthSales);
            return summarySales;
        }

        public async Task<List<SaleReportDetails>> GetSaleReports(int branchId)
        {
            var saleReports = await _context.SaleReportDetails.Where(sr => sr.BranchId == branchId)
                .OrderByDescending(sr => sr.DateReported)
                .ToListAsync();

            return saleReports;
        }
        public async Task AddSaleReport(SaleReportDto saleReportDto, int branchId)
        {
            var newSaleReport = new SaleReportDetails()
            {
                BranchId = branchId,
                DateSale = saleReportDto.DateSale,
                TotalSales = saleReportDto.TotalSale,
                SaleType = saleReportDto.SaleType,
                DateReported = DateTime.Now
            };
            _context.SaleReportDetails.Add(newSaleReport);
            await _context.SaveChangesAsync();

        }

        public async Task<SaleDto> GetTotalSales(int branchId, int year)
        {
            var saleDto = new SaleDto();
            saleDto.BranchId = branchId;
            saleDto.Sales = new List<TotalSaleDto>();

            for (int i = 1; i <= 12; i++)
            {
                var sales = await _context.Sales.Where(sale => sale.BranchId == branchId && sale.DateSale.Year == year && sale.DateSale.Month == i)
                    .ToListAsync();
                if (sales.Count > 0)
                {
                    double totalSale = sales.Sum(sale => sale.TotalSale);
                    saleDto.Sales.Add(new TotalSaleDto() { DateSale = sales.FirstOrDefault().DateSale, TotalSale = (decimal)totalSale });
                }

            }
            return saleDto;
        }
        public async Task<double> GetSales(int branchId, DateTime date, string filterBy)
        {
            double totalSale = 0;
            switch (filterBy)
            {
                case "daily":
                    totalSale = await GetDailySales(branchId, date);
                    return totalSale;
                case "monthly":
                    totalSale = await GetMonthlySale(branchId, date);
                    return totalSale;
                default:
                    return totalSale;
            }
        }
        private async Task<double> GetDailySales(int branchId, DateTime date)
        {
            double dailySale = await _context.Sales.Where(sale => sale.BranchId == branchId
                && sale.DateSale.Year == date.Year
                && sale.DateSale.Month == date.Month
                && sale.DateSale.Day == date.Day)
                .SumAsync(sale => sale.TotalSale);
            return dailySale;
        }
        private async Task<double> GetMonthlySale(int branchId, DateTime date)
        {
            double monthlySale = await _context.Sales.Where(sale => sale.BranchId == branchId
                && sale.DateSale.Year == date.Year
                && sale.DateSale.Month == date.Month)
                .SumAsync(sale => sale.TotalSale);
            return monthlySale;
        }
        public async Task<double> GetTodaySales(int branchId)
        {
            var totalSales = await _context.Sales.Where(sale => sale.BranchId == branchId
            && sale.DateSale.Year == DateTime.Now.Year
            && sale.DateSale.Month == DateTime.Now.Month
            && sale.DateSale.Day == DateTime.Now.Day)
                .SumAsync(sale => sale.TotalSale);
            return totalSales;
        }

        public async Task<List<Sale>> GetBestSellingProduct(int branchId, DateTime dateTime, string filterBy)
        {
            List<Sale> bestSellingProducts = null;
            switch (filterBy)
            {
                case "daily":
                    bestSellingProducts = await GetBestSellingDaily(branchId, dateTime);
                    return bestSellingProducts;
                case "monthly":
                    bestSellingProducts = await GetBestSellingMonthly(branchId, dateTime);
                    return bestSellingProducts;
                default:
                    return bestSellingProducts;
            }
        }
        private async Task<List<Sale>> GetBestSellingMonthly(int branchId, DateTime dateTime)
        {
            var bestSellingProducts = new List<Sale>();
            var saleProducts = await _context.Sales.Where(sale => sale.BranchId == branchId
                && sale.DateSale.Year == dateTime.Year
                && sale.DateSale.Month == dateTime.Month)
                .Include(sale => sale.Branch)
                .Include(sale => sale.HardwareProduct)
                .ToListAsync();

            foreach(var saleProduct in saleProducts)
            {
                
                var getSaleProduct = bestSellingProducts.FirstOrDefault(sale => sale?.HardwareProductId == saleProduct.HardwareProductId);
                if(getSaleProduct == null)
                {
                    var getSaleProducts = saleProducts.Where(sale => sale.HardwareProductId == saleProduct.HardwareProductId)
                        .ToList();
                    saleProduct.TotalSale = getSaleProducts.Sum(s => s.TotalSale);
                    bestSellingProducts.Add(saleProduct);
                }
                //if (getSaleProduct != null)
                //{
                //    int index = bestSellingProducts.FindIndex(s => s.HardwareProductId == getSaleProduct.HardwareProductId);
                //    bestSellingProducts[index].TotalSale += getSaleProduct.TotalSale;
                //}
                //else
                //{
                //    bestSellingProducts.Add(saleProduct);
                //}
            }

            bestSellingProducts = bestSellingProducts.OrderByDescending(sale => (sale.TotalSale / (double)sale.HardwareProduct.CostPrice)).ToList();
            return bestSellingProducts;
        }

        private async Task<List<Sale>> GetBestSellingDaily(int branchId, DateTime dateTime)
        {
            var bestSellingProducts = await _context.Sales.Where(sale => sale.BranchId == branchId
                && sale.DateSale.Year == dateTime.Year
                && sale.DateSale.Month == dateTime.Month
                && sale.DateSale.Day == dateTime.Day)
                .Include(sale => sale.Branch)
                .Include(sale => sale.HardwareProduct)
                .OrderByDescending(sale => (sale.TotalSale / (double)sale.HardwareProduct.CostPrice))
                .ToListAsync();


            return bestSellingProducts;
        }
    }
}
