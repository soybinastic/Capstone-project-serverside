using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ProfitOption _profitOption;
        public DashboardRepository(ApplicationDbContext db, IOptions<ProfitOption> profitOption)
        {
            _db = db;
            _profitOption = profitOption.Value;
        }
        public async Task<List<Dashboard>> GetAll()
        {
            // DateTime datenow = DateTime.Now;
            // var dashboard = await _db.Dashboard.Where(d => (d.Date.Year == datenow.Year
            //     && d.Date.Month == datenow.Month) ||  d.Status == Keyword.UN_PAID)
            //     .OrderBy(d => d.Status == Keyword.UN_PAID)
            //     .ThenByDescending(d => d.Total)
            //     .Include(d => d.Branch)
            //     .ToListAsync();
            var dashboard = await _db.Dashboard.Where(d => d.Status == Keyword.ON_GOING ||  d.Status == Keyword.UN_PAID)
                .OrderBy(d => d.Status == Keyword.UN_PAID)
                .ThenByDescending(d => d.Total)
                .Include(d => d.Branch)
                .ToListAsync();
            
            return dashboard;
        }

        public async Task<List<Dashboard>> GetAll(int branchId)
        {
            var dashboards = await _db.Dashboard
                .Include(d => d.Branch)
                .Where(d => d.BranchId == branchId)
                .OrderBy(d => d.Date.Date)
                .ToListAsync();
            
            return dashboards;
        }
        public async Task<Dashboard> GetById(int dashboardId)
        {
            var dashboard = await _db.Dashboard
                .Include(d => d.Branch)
                .Where(d => d.Id == dashboardId)
                .FirstOrDefaultAsync();

            return dashboard;
        }

        public async Task<List<Dashboard>> ToPay(int branchId)
        {
            DateTime datenow = DateTime.Now;

            var dashboard = await _db.Dashboard.Where(d => d.BranchId == branchId && (d.Date.Year == datenow.Year
                && d.Date.Month == datenow.Month) ||  d.Status == Keyword.UN_PAID)
                .OrderBy(d => d.Status == Keyword.UN_PAID)
                .ThenByDescending(d => d.Total)
                .Include(d => d.Branch)
                .ToListAsync();
            
            return dashboard;
        }

        public async Task Upsert(List<CustomerOrderProduct> orderItems, int branchId, DateTime datenow)
        {
            // to be continued.
            // to ensure the function.
            // not totally fix yet.
            const double minimumsalesOfMonth = 5000;
            
            
            var branch = await _db.Branches.Where(b => b.Id == branchId).FirstOrDefaultAsync();

            if(branch != null)
            {
                double sales = orderItems.Sum(oi => oi.ProductPrice * oi.ProductQuantity);
                double profit = orderItems.Sum(oi => (oi.ProductPrice * oi.ProductQuantity) * _profitOption.Value);

                var dashboardSale = await _db.Dashboard
                    .Include(d => d.Branch)
                    .Where(d => d.BranchId == branchId
                    && datenow.Date <= d.DueDate.Date)
                    .FirstOrDefaultAsync();
            
                if(dashboardSale != null)
                {
                    dashboardSale.SalesOfMonth += sales;
                    dashboardSale.OriginalSales = dashboardSale.SalesOfMonth - (dashboardSale.SalesOfMonth * _profitOption.Value);
                    dashboardSale.PlatformFee = (dashboardSale.SalesOfMonth > minimumsalesOfMonth) ? 1000 : 0;
                    dashboardSale.Profit += profit;
                    dashboardSale.Total += profit;
                    await _db.SaveChangesAsync();
                }
                else
                {   
                    dashboardSale = new Dashboard 
                    {
                        BranchId = branchId,
                        Branch = branch,
                        DueDate = DateTimeHelper.GetDueDate(branch.DateRegistered, datenow).AddDays(-1),
                        PlatformFee = (sales > minimumsalesOfMonth) ? 1000 : 0,
                        SalesOfMonth = sales,
                        OriginalSales = sales - (sales * _profitOption.Value),
                        Profit = profit,
                        Total = profit,
                        Status = Keyword.ON_GOING,
                        Date = DateTime.Now
                    };

                    await _db.Dashboard.AddAsync(dashboardSale);
                    await _db.SaveChangesAsync();
                }
            }
        }

    }
}