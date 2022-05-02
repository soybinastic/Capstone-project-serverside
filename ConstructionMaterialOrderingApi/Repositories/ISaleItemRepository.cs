using ConstructionMaterialOrderingApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.SaleItemDtos;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface ISaleItemRepository
    {
        Task<List<SaleItemDto>> GetSaleItems(int branchId, DateTime date, string filterBy);
    }
}