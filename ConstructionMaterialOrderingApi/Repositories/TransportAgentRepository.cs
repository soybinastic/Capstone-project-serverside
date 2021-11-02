using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Dtos.TransportAgentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class TransportAgentRepository : ITransportAgentRepository
    {
        private readonly ApplicationDbContext _context;

        public TransportAgentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddTransportAgent(CreateTransportAgentDto model, int hardwareStoreId, string accountId)
        {
            var transportAgent = new TransportAgent()
            {
                AccountId = accountId,
                HardwareStoreId = hardwareStoreId,
                Name = $"{model.FirstName} {model.LastName}"
            };
            await _context.TransportAgents.AddAsync(transportAgent);
            await _context.SaveChangesAsync();
            
        }

        public async Task<GetTransportAgentDto> GetTransportAgentByAccountId(string accountId)
        {
            var transportAgent = await _context.TransportAgents.Where(t => t.AccountId == accountId)
                .FirstOrDefaultAsync();

            var transportAgentDto = new GetTransportAgentDto()
            {
                TransportAgentId = transportAgent.Id,
                AccountId = transportAgent.AccountId,
                HardwareStoreId = transportAgent.HardwareStoreId,
                Name = transportAgent.Name
            };

            return transportAgentDto;
        }
    }
}
