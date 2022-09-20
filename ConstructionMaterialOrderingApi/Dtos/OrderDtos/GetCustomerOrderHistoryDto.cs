using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.OrderDtos
{
    public class GetCustomerOrderHistoryDto
    {
        // public int CustomerOrderHistoryId { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int HardwareStoreId { get; set; }
        // public string HardwareStoreName { get; set; }
        public DateTime OrderDate { get; set; }
        public double Total { get; set; }
        public bool Deliver { get; set; }
        public bool IsRecieved { get; set; }
        public string BranchName { get; set; }
        public int BrancId { get; set; }
        public string Status { get; set; }
        public double ShippingFee { get; set; }
    }
}
