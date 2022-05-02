using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;
using ConstructionMaterialOrderingApi.Dtos.SaleItemDtos;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class SaleItemRepository : ISaleItemRepository
    {
        private readonly ApplicationDbContext _context;
        public SaleItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SaleItemDto>> GetSaleItems(int branchId, DateTime date, string filterBy)
        {
            
            switch(filterBy)
            {
                case "daily":
                    return await Daily(branchId, date);
                case "monthly":
                    return await Monthly(branchId, date);
                case "yearly":
                    return await Yearly(branchId, date);
                default:
                    return null;
            };
            

            //saleItems.ForEach((saleItem) => 
            //    {
            //        var summaries = _context.SaleItemSummaries.Where(s => s.SaleItemId == saleItem.Id)
            //            .ToList();
            //        List<SaleItemSummary> summarylist = new List<SaleItemSummary>();
            //        summaries.ForEach((summary) => 
            //            {

            //                summarylist.Add(new SaleItemSummary
            //                {
            //                    Id = summary.Id,
            //                    Amount = summary.Amount,
            //                    SaleItemId = summary.SaleItemId,
            //                    HardwareProductId = summary.HardwareProductId,
            //                    HardwareProduct = _context.HardwareProducts.Where(p => p.Id == summary.HardwareProductId).FirstOrDefault(),
            //                    SaleItem = _context.SaleItems.Where(s => s.Id == summary.SaleItemId).FirstOrDefault()

            //                });
            //            });

            //        saleItemsList.Add(new SaleItem
            //            {
            //                Id = saleItem.Id,
            //                BranchId = saleItem.BranchId,
            //                Branch = saleItem.Branch,
            //                CustomerId = saleItem.CustomerId,
            //                Customer = saleItem.Customer,
            //                SaleItemDate = saleItem.SaleItemDate,
            //                Total = saleItem.Total,
            //                SaleItemSummaries = summarylist
            //            });
            //    });

            
        }
        private async Task<List<SaleItemDto>> Yearly(int branchId, DateTime date)
        {
            var saleItems = await _context.SaleItems.Where(saleItem => saleItem.BranchId == branchId
                && saleItem.SaleItemDate.Year == date.Year)
                .Include(saleItem => saleItem.Branch)
                .Include(saleItem => saleItem.Customer)
                .Select(s => new SaleItemDto
                {
                    Id = s.Id,
                    BranchId = s.BranchId,
                    Branch = s.Branch,
                    CustomerId = s.CustomerId,
                    Customer = s.Customer,
                    Total = s.Total,
                    OR = s.ORNumber,
                    SaleItemDate = s.SaleItemDate,
                    SaleItemSummaries = _context.SaleItemSummaries.Where(sm => sm.SaleItemId == s.Id)
                            .Select(sm => new SaleItemSummaryDto
                            {
                                Id = sm.Id,
                                SaleItemId = sm.SaleItemId,
                                HardwareProductId = sm.HardwareProductId,
                                Amount = sm.Amount,
                                HardwareProduct = _context.HardwareProducts.FirstOrDefault(p => p.Id == sm.HardwareProductId)
                            })
                            .ToList()
                })
                .OrderBy(saleItem => saleItem.SaleItemDate)
                .ToListAsync();
            return saleItems;
        }
        private async Task<List<SaleItemDto>> Monthly(int branchId, DateTime date)
        {
            var saleItems = await _context.SaleItems.Where(saleItem => saleItem.BranchId == branchId
                && saleItem.SaleItemDate.Year == date.Year
                && saleItem.SaleItemDate.Month == date.Month)
                .Include(saleItem => saleItem.Branch)
                .Include(saleItem => saleItem.Customer)
                .Select(s => new SaleItemDto
                {
                    Id = s.Id,
                    BranchId = s.BranchId,
                    Branch = s.Branch,
                    CustomerId = s.CustomerId,
                    Customer = s.Customer,
                    Total = s.Total,
                    OR = s.ORNumber,
                    SaleItemDate = s.SaleItemDate,
                    SaleItemSummaries = _context.SaleItemSummaries.Where(sm => sm.SaleItemId == s.Id)
                            .Select(sm => new SaleItemSummaryDto
                            {
                                Id = sm.Id,
                                SaleItemId = sm.SaleItemId,
                                HardwareProductId = sm.HardwareProductId,
                                Amount = sm.Amount,
                                HardwareProduct = _context.HardwareProducts.FirstOrDefault(p => p.Id == sm.HardwareProductId)
                            })
                            .ToList()
                })
                .OrderBy(saleItem => saleItem.SaleItemDate)
                .ToListAsync();
            return saleItems;
        }
        private async Task<List<SaleItemDto>> Daily(int branchId, DateTime date)
        {
            var saleItems = await _context.SaleItems.Where(saleItem => saleItem.BranchId == branchId
                && saleItem.SaleItemDate.Year == date.Year
                && saleItem.SaleItemDate.Month == date.Month
                && saleItem.SaleItemDate.Day == date.Day)
                .Include(saleItem => saleItem.Branch)
                .Include(saleItem => saleItem.Customer)
                .Select(s => new SaleItemDto
                {
                    Id = s.Id,
                    BranchId = s.BranchId,
                    Branch = s.Branch,
                    CustomerId = s.CustomerId,
                    Customer = s.Customer,
                    Total = s.Total,
                    OR = s.ORNumber,
                    SaleItemDate = s.SaleItemDate,
                    SaleItemSummaries = _context.SaleItemSummaries.Where(sm => sm.SaleItemId == s.Id)
                            .Select(sm => new SaleItemSummaryDto
                            {
                                Id = sm.Id,
                                SaleItemId = sm.SaleItemId,
                                HardwareProductId = sm.HardwareProductId,
                                Amount = sm.Amount,
                                HardwareProduct = _context.HardwareProducts.FirstOrDefault(p => p.Id == sm.HardwareProductId)
                            })
                            .ToList()
                })
                .ToListAsync();
            return saleItems;
        }
    }
}
