using ConstructionMaterialOrderingApi.Dtos.RequestDtos;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IRequestProductRepository
    {
        Task Add(int requestId, RequestProductDto requestProductDto);
        Task MakeCompleteRequestProduct(int hardwareProductId, int requestProductId);
    }
}