using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class ConfirmedOrder
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int HardwareStoreUserId { get; set; }
        public HardwareStoreUser ConfirmedBy { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime DateConfirmed { get; set; }
    }
}
