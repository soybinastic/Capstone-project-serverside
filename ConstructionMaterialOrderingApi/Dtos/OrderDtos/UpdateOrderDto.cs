using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.OrderDtos
{
    public class UpdateOrderDto
    {
        public int OrderId { get; set; }
        public int HardwareStoreId { get; set; }
        public int CustomerId { get; set; }
        public int BranchId { get; set; }
        public bool IsCancelled { get; set; }
    }
}
