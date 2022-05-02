using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.SaleDtos
{
    public class BestSellingDetailDto
    {
        public DateTime BestSellingDate { get; set; }
        public List<BestSellingProductDto> BestSellingProducts { get; set; }
    }
}
