using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.SaleDtos
{
    public class TotalSaleDto
    {
        public DateTime DateSale { get; set; }
        public decimal TotalSale { get; set; }
    }
}
