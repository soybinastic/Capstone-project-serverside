using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class WarehouseProduct
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int HardwareProductId { get; set; }
        public HardwareProduct HardwareProduct { get; set; }
        public int StockNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
