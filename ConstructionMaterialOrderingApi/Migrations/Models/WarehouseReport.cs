using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class WarehouseReport
    {
        public int Id { get; set; }
        public int HardwareStoreId { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public string Description { get; set; }
        public string ReportType { get; set; }
        public DateTime DateReported { get; set; }
    }
}
