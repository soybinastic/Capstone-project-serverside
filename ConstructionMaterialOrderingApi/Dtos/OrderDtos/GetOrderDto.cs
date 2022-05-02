using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.OrderDtos
{
    public class GetOrderDto
    {
        public int OrderId { get; set; }
        public int HardwareStoreId { get; set; }
        public int BranchId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public double Total { get; set; }
        public bool Deliver { get; set; }
        public bool IsCustomerOrderRecieved { get; set; }
        public bool IsOrderCanceled { get; set; }
        public string Status { get; set; }
        public bool IsApproved { get; set; }
    }
}
