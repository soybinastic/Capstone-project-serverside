using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class SaleItemSummary
    {
        public int Id { get; set; }
        public int SaleItemId { get; set; }
        public SaleItem SaleItem { get; set; }
        public int HardwareProductId { get; set; }
        public HardwareProduct HardwareProduct { get; set; }
        public double Amount { get; set; }
    }
}
