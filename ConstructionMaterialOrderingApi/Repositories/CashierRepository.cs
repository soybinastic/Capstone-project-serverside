using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class CashierRepository : IBranchUserRepository<Cashier>
    {
        private readonly ApplicationDbContext _db;
        public CashierRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Cashier> Add(Cashier newBranchUser)
        {
            var result = await _db.Cashiers.AddAsync(newBranchUser);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Cashier> GetByAccountId(string accountId)
        {
            var cashier = await _db.Cashiers.FirstOrDefaultAsync(c => c.AccountId == accountId);
            return cashier;
        }
    }
}