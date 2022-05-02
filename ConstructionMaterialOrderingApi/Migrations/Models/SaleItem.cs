using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class SaleItem
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime SaleItemDate { get; set; } 
        [ForeignKey("SaleItemId")]
        public List<SaleItemSummary> SaleItemSummaries { get; set; }
        public double Total { get; set; }
        public string ORNumber { get; set; }
    }
}
