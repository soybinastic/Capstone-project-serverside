using ConstructionMaterialOrderingApi.Dtos.RecieveProductDtos;
using ConstructionMaterialOrderingApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IRecieveProductRepository
    {
        Task Add(RecieveProductDto recieveProductDto, int warehouseReportId, int warehouseId);
        Task<RecieveProduct> GetRecieveProduct(int recieveProductId);
        Task<List<RecieveProduct>> GetRecieveProducts(int warehouseReportId);
        Task Update(RecieveProductDto recieveProductDto, int recieveProductId, int warehouseId);
    }
}