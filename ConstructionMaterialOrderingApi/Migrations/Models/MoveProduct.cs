using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class MoveProduct
    {
        public int Id { get; set; }
        public int HardwareStoreId { get; set; }
        public int FromWarehouseId { get; set; }
        public int HardwareProductId { get; set; }
        public HardwareProduct Product { get; set; }
        public int MoveToWarehouseId { get; set; }
        public int Quantity { get; set; }
        public DateTime MoveDate { get; set; }
    }
}
