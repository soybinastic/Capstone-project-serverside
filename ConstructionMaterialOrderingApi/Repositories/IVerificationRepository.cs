using ConstructionMaterialOrderingApi.Dtos.CustomerVerificationDtos;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IVerificationRepository
    {
        Task<CustomerVerification> Get(int id);
        Task<List<CustomerVerification>> GetAll();
        Task<(bool, string)> Post(VerificationDetail verificationDetail, int customerId);
        Task<(bool, string)> VerifyCustomer(int customerId);
    }
}