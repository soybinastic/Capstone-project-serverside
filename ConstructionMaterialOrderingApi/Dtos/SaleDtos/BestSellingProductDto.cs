using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.SaleDtos
{
    public class BestSellingProductDto
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int HardwareProductId { get; set; }
        public double TotalSale { get; set; }
        public DateTime DateSale { get; set; }
    }
}
