using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.SaleDtos
{
    public class SaleDto
    {
        public int BranchId { get; set; }
        public List<TotalSaleDto> Sales { get; set; }
    }
}
