using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public int HardwareProductId { get; set; }
        public HardwareProduct HardwareProduct { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double TotalSale { get; set; }
        public DateTime DateSale { get; set; }
    }
}
