using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class HardwareStore
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string HardwareStoreName { get; set; }
        public string Owner { get; set; }
        public string BusinessAddress { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public bool IsOpen { get; set; }

    }
}
