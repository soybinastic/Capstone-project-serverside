using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.TransportAgentDtos;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface ITransportAgentRepository
    {
        Task AddTransportAgent(CreateTransportAgentDto model, int hardwareStoreId, string accountId);
        Task<GetTransportAgentDto> GetTransportAgentByAccountId(string accountId);
    }
}
