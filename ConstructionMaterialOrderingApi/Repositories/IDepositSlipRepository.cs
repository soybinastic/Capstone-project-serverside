using ConstructionMaterialOrderingApi.Dtos.DepositSlipDtos;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IDepositSlipRepository
    {
        Task Add(DepositSlipDto depositSlipDto, int branchId);
        Task<DepositSlip> GetDepositSlip(int branchId, int depositSlipId);
        Task<List<DepositSlip>> GetDepositSlips(int branchId);
        Task<bool> Update(int branchId, int depositSlipId, DepositSlipDto depositSlipDto);
    }
}