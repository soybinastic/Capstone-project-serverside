using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class RequestProduct
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int HardwareProductId { get; set; }
        public HardwareProduct HardwareProduct { get; set; }
        public int Quantity { get; set; }
        public bool IsComplete { get; set; }
    }
}
