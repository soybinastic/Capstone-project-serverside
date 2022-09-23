using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class SalesClerkRepository : IBranchUserRepository<SalesClerk>
    {
        private readonly ApplicationDbContext _db;
        public SalesClerkRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<SalesClerk> Add(SalesClerk newBranchUser)
        {
            var result = await _db.SalesClerks.AddAsync(newBranchUser);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<SalesClerk> GetByAccountId(string accountId)
        {
            return await _db.SalesClerks.FirstOrDefaultAsync(sc => sc.AccountId == accountId);
        }
    }
}