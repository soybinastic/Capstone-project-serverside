using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Dtos.TransportAgentDtos;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface ITransportAgentRepository
    {
        Task AddTransportAgent(CreateTransportAgentDto model, int hardwareStoreId, string accountId);
        Task<GetTransportAgentDto> GetTransportAgentByAccountId(string accountId);
        Task<TransportAgent> GetTransportAgentByAccountID(string accountId);
        Task AddTransportAgent(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId);
    }
}
