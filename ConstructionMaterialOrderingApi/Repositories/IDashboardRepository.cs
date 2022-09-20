
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Helpers;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IDashboardRepository
    {
        Task Upsert(List<CustomerOrderProduct> orderItems, int branchId, DateTime datenow);
        Task<List<Dashboard>> GetAll();
        Task<List<Dashboard>> GetAll(int branchId);
        Task<List<Dashboard>> ToPay(int branchId);
        Task<Dashboard> GetById(int dashboardId);
        Task UpdateStatus(int dashboardId, string status = Keyword.PAID);
    }
}