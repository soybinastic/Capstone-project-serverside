using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IRecipientRepository
    {
        Task<List<Recipient>> GetAllRecipientsByBranchId(int branchId);
        Task<List<Recipient>> GetAlRecipientsByCustomerId(int customerId);
        Task<Recipient> GetRecipient(int recipientId);
        Task<Recipient> GetRecipientByOrderId(int orderId);
    }
}