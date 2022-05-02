using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Dtos.DeliverProductDtos
{
    public class DeliverProductReportDto
    {
        public WarehouseReport ReportDetails { get; set; }
        public List<DeliverProduct> DeliverProducts { get; set; }
    }
}
