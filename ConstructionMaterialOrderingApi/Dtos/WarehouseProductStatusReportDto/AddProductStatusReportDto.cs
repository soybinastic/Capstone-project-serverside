using ConstructionMaterialOrderingApi.Dtos.WarehouseReportDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.WarehouseProductStatusReportDto
{
    public class AddProductStatusReportDto
    {
        [Required]
        public WarehouseReportDto ReportDetail { get; set; }
        [Required]
        public List<ProductStatusReportDto> ProductStatusReports { get; set; }
    }
}
