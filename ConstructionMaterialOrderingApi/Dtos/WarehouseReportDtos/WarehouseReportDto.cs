using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.WarehouseReportDtos
{
    public class WarehouseReportDto
    {
        public string Description { get; set; }
        [Required]
        public string ReportType { get; set; }
    }
}
