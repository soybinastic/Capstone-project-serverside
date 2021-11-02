using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class CustomerOrderHistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int HardwareStoreId { get; set; }
        public string HardwareStoreName { get; set; }
        public DateTime OrderDate { get; set; }
        public double Total { get; set; }
        public bool Deliver { get; set; }
        public bool IsRecieved { get; set; }
    }
}
