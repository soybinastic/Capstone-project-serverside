using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.TransportAgentDtos
{
    public class GetTransportAgentDto
    {
        public int TransportAgentId { get; set; }
        public string AccountId { get; set; }
        public int HardwareStoreId { get; set; }
        public string Name { get; set; }
    }
}
