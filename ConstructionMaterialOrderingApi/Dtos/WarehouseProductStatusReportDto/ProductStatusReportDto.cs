using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.WarehouseProductStatusReportDto
{
    public class ProductStatusReportDto
    {
        public int HardwareProductId { get; set; }
        public string Description { get; set; }
        public string StatusType { get; set; }
    }
}
