using ConstructionMaterialOrderingApi.Dtos.WarehouseReportDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.RecieveProductDtos
{
    public class AddRecieveProductDto
    {
        [Required]
        public WarehouseReportDto ReportDetail { get; set; }
        [Required]
        public List<RecieveProductDto> RecieveProducts { get; set; }
    }
}
