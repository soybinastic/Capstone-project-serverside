using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int HardwareStoreId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public double Total { get; set; }
        public bool Deliver { get; set; }
        public bool IsCustomerOrderRecieved { get; set; }
    }
}
