using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class RecipientRepository : IRecipientRepository
    {
        private readonly ApplicationDbContext _context;
        public RecipientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipient>> GetAllRecipientsByBranchId(int branchId)
        {
            var recipients = await _context.Recipients.Where(r => r.BranchId == branchId)
                .Include(r => r.Branch)
                .Include(r => r.Customer)
                .Include(r => r.RecipientItems)
                .Include(r => r.RecipientItems.Select(p => p.HardwareProduct))
                .Include(r => r.RecipientItems.Select(r => r.Recipient))
                .ToListAsync();

            return recipients;
        }
        public async Task<Recipient> GetRecipientByOrderId(int orderId)
        {
            var recipient = await _context.Recipients.Where(r => r.OrderId == orderId)
                .Include(r => r.Branch)
                .Include(r => r.Customer)
                .Include(r => r.RecipientItems)
                .Include(r => r.RecipientItems.Select(p => p.HardwareProduct))
                .Include(r => r.RecipientItems.Select(r => r.Recipient))
                .FirstOrDefaultAsync();

            return recipient;
        }
        public async Task<Recipient> GetRecipient(int recipientId)
        {
            var recipient = await _context.Recipients.Where(r => r.Id == recipientId)
                .Include(r => r.Branch)
                .Include(r => r.Customer)
                .Include(r => r.RecipientItems)
                .Include(r => r.RecipientItems.Select(p => p.HardwareProduct))
                .Include(r => r.RecipientItems.Select(r => r.Recipient))
                .FirstOrDefaultAsync();

            return recipient;
        }
        public async Task<List<Recipient>> GetAlRecipientsByCustomerId(int customerId)
        {
            var recipients = await _context.Recipients.Where(r => r.CustomerId == customerId)
                .Include(r => r.Branch)
                .Include(r => r.Customer)
                .Include(r => r.RecipientItems)
                .Include(r => r.RecipientItems.Select(p => p.HardwareProduct))
                .Include(r => r.RecipientItems.Select(r => r.Recipient))
                .ToListAsync();

            return recipients;
        }
    }
}
