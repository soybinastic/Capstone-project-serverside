using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.SaleDtos
{
    public class SaleReportDto
    {
        public string SaleType { get; set; }
        public double TotalSale { get; set; }
        public DateTime DateSale { get; set; }
    }
}
