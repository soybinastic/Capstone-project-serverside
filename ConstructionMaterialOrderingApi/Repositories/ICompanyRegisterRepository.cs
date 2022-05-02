using ConstructionMaterialOrderingApi.Dtos.CompanyRegisterDtos;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface ICompanyRegisterRepository
    {
        Task<bool> Delete(int companyRegisterId);
        Task<CompanyRegister> Get(int companyRegisterId);
        Task<List<CompanyRegister>> GetAll();
        Task Register(RegisterDto registerDto);
    }
}