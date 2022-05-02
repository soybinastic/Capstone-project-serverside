using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class BestSellingProductReport
    {
        public int Id { get; set; }
        public int BestSellingDetailId { get; set; }
        public int SaleId { get; set; }
        public Sale Sale { get; set; }
    }
}
