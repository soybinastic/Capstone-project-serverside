
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class ConfirmedOrderRepository : IConfirmedOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public ConfirmedOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(ConfirmedOrder confirmedOrder)
        {
            await _context.ConfirmedOrders.AddAsync(confirmedOrder);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ConfirmedOrder>> GetConfirmedOrders(int branchId)
        {
            var confirmedOrders = await _context.ConfirmedOrders.Where(co => co.BranchId == branchId)
                    .Include(co => co.Customer)
                    .Include(co => co.ConfirmedBy)
                    .Include(co => co.Order)
                    .OrderByDescending(co => co.DateConfirmed)
                    .ToListAsync();

            return confirmedOrders;
        }
        public async Task<ConfirmedOrder> GetConfirmedOrder(int branchId, int orderId)
        {
            var confirmedOrder = await _context.ConfirmedOrders.Where(co => co.BranchId == branchId && co.OrderId == orderId)
                    .FirstOrDefaultAsync();
            return confirmedOrder;
        }

        private async Task<string> OrGenerator(int branchId)
        {
            Random rnd1 = new Random();
            Random rnd2 = new Random();
            var count = await _context.SaleItems.Where(s => s.BranchId == branchId).CountAsync();
            int ranNum1 = rnd1.Next(1,10);
            int ranNum2 = rnd2.Next(10,20);

            //string returnValue = $"{ranNum1}{ranNum2}{DateTime.Now.Year}{count + 1}";
            StringBuilder returnVal = new StringBuilder();

            returnVal.Append(ranNum1.ToString());
            returnVal.Append(ranNum2.ToString());
            returnVal.Append(DateTime.Now.Year.ToString());
            returnVal.Append((count + 1).ToString());

            return returnVal.ToString();
        }

        public async Task<bool> ConfirmOrder(int orderId, int branchId)
        {
            DateTime datenow = DateTime.Now;
            var order = await _context.Orders.Where(o => o.Id == orderId && o.BranchId == branchId)
                .FirstOrDefaultAsync();
            if (order == null)
                return false;

            var orderProducts = await _context.CustomerOrderProducts.Where(op => op.OrderId == order.Id)
                .ToListAsync();

            var newSaleItem = new SaleItem()
            {
                BranchId = branchId,
                CustomerId = order.CustomerId,
                SaleItemDate = datenow,
                Total = order.Total,
                ORNumber = await OrGenerator(branchId)
            };

            _context.SaleItems.Add(newSaleItem);
            await _context.SaveChangesAsync();

            var newRecipient = new Recipient
            {
                OrderId = orderId,
                BranchId = branchId,
                CustomerId = order.CustomerId,
                TotalAmount = order.Total,
                Date = datenow
            };

            _context.Recipients.Add(newRecipient);
            await _context.SaveChangesAsync();

            foreach (var product in orderProducts)
            {
                var sale = await _context.Sales.Where(sale => sale.DateSale.Year == datenow.Year
                    && sale.DateSale.Month == datenow.Month
                    && sale.DateSale.Day == datenow.Day
                    && sale.BranchId == branchId
                    && sale.HardwareProductId == product.ProductId)
                    .FirstOrDefaultAsync();

                if (sale != null)
                {
                    sale.TotalSale += (product.ProductQuantity * product.ProductPrice);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newSale = new Sale()
                    {
                        BranchId = branchId,
                        HardwareProductId = product.ProductId,
                        TotalSale = product.ProductQuantity * product.ProductPrice,
                        DateSale = DateTime.Now
                    };

                    await _context.Sales.AddAsync(newSale);
                    await _context.SaveChangesAsync();
                }

                var newSaleItemSummary = new SaleItemSummary
                {
                    SaleItemId = newSaleItem.Id,
                    HardwareProductId = product.ProductId,
                    Amount = (product.ProductQuantity * product.ProductPrice)
                };

                await _context.SaleItemSummaries.AddAsync(newSaleItemSummary);
                await _context.SaveChangesAsync();

                var newRecipientItem = new RecipientItem
                {
                    RecipientId = newRecipient.Id,
                    HardwareProductId = product.ProductId,
                    Amount = (product.ProductQuantity * product.ProductPrice)
                };

                await _context.RecipientItems.AddRangeAsync(newRecipientItem);
                await _context.SaveChangesAsync();
            }

            var confirmOrder = await _context.ConfirmedOrders
                .Where(co => co.OrderId == order.Id && co.BranchId == branchId)
                .FirstOrDefaultAsync();

            confirmOrder.IsConfirmed = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
