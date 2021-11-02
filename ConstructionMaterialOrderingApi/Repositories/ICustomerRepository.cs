using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.CustomerDtos;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface ICustomerRepository
    {
        Task RegisterCustomer(CustomerRegisterDto model, string accountId);
        Task<GetCustomerDto> GetCustomerByAccountId(string customerAccountId);
    }
}
