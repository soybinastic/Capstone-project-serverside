using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.OrderDtos;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class OrderPreparationRepository : IOrderPreparationRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderPreparationRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<OrderToPrepare>> GetAll(int salesClerkId)
        {
            var orderToPrepares = await _db.OrderToPrepares
                .Include(otp => otp.Order)
                .Include(otp => otp.SalesClerk)
                .Where(otp => otp.SalesClerkId == salesClerkId)
                .ToListAsync();
            return orderToPrepares;
        }

        public async Task<IEnumerable<AvailableSalesClerk>> GetAvailableSalesClerk(int branchId)
        {
            var salesClerks = await _db.SalesClerks.Where(sc => sc.BranchId == branchId)
                .Select(s => new AvailableSalesClerk
                    {
                        SalesClerk = s,
                        IsAvailable = !_db.OrderToPrepares.Include(otp => otp.Order)
                                            .Where(otp => otp.SalesClerkId == s.Id)
                                            .Any(otp => otp.Order.Status == OrderStatus.PREPARING)
                    }).ToListAsync();
            return salesClerks;
        }

        public async Task<bool> SalesClerkAvailable(int salesClerkId)
        {
            var isNotAvailable = await _db.OrderToPrepares.Include(otp => otp.Order)
                .Where(otp => otp.SalesClerkId == salesClerkId)
                .AnyAsync(otp => otp.Order.Status == OrderStatus.PREPARING);
            return isNotAvailable ? false : true;
        }

        public async Task<OrderToPrepare> ToDeliver(int orderId)
        {
            var orderToPrepare = await _db.OrderToPrepares.Include(otp => otp.Order)
                .FirstOrDefaultAsync(otp => otp.OrderId == orderId);
            if(orderToPrepare == null) return null;
            orderToPrepare.Order.Status = OrderStatus.TO_DELIVER;
            await _db.SaveChangesAsync();
            return orderToPrepare;
        }
    }
}