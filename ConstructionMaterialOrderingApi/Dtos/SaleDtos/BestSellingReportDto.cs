using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.SaleDtos
{
    public class BestSellingReportDto
    {
        public int WarehouseId { get; set; }
        public string BestSellingType { get; set; }
        public List<BestSellingDetailDto> BestSellingDetails { get; set; }

    }
}
