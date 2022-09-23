using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Models;
namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IBranchUserRepository<T> where T : class
    {
        Task<T> GetByAccountId(string accountId);
        Task<T> Add(T newBranchUser);
    }
}