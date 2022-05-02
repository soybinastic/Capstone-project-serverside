using ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto;
using ConstructionMaterialOrderingApi.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Implementations
{
    public class TransportAgent : IHardwareStoreUser
    {
        private readonly ITransportAgentRepository _transportAgentRepository;

        public TransportAgent(ITransportAgentRepository transportAgentRepository)
        {
            _transportAgentRepository = transportAgentRepository;
        }
        public void CreateUser(HardwareStoreUserDto storeUserDto, string applicationUserId, int hardwareStoreId)
        {
            _transportAgentRepository.AddTransportAgent(storeUserDto, applicationUserId, hardwareStoreId);
        }

        public async Task<UserInformationDto> GetUser(string accountId)
        {
            var transportAgent = await _transportAgentRepository
                                .GetTransportAgentByAccountID(accountId);
            var userInformation = new UserInformationDto()
            {
                Id = transportAgent.Id,
                AccountId = transportAgent.AccountId,
                HardwareStoreId = transportAgent.HardwareStoreId,
                BranchId = transportAgent.BranchId,
                Name = transportAgent.Name,
                Role = "TransportAgent"
            };
            return userInformation;
        }
    }
}
