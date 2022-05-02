using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class WarehouseProductStatusReport
    {
        public int Id { get; set; }
        public int WarehouseReportId { get; set; }
        public WarehouseReport WarehouseReport { get; set; }
        public int HardwareProductId { get; set; }
        public HardwareProduct HardwareProduct { get; set; }
        public string Description { get; set; }
        public string StatusType { get; set; }
    }
}
