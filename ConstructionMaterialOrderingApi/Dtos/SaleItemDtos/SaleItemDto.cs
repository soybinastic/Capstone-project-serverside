using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Dtos.SaleItemDtos
{
    public class SaleItemDto
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime SaleItemDate { get; set; }
        public List<SaleItemSummaryDto> SaleItemSummaries { get; set; }
        public double Total { get; set; }
        public string OR { get; set; }
    }
}
