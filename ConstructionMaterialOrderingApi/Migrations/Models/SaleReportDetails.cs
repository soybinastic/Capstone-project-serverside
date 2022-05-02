using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class SaleReportDetails
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public DateTime DateSale { get; set; }
        public DateTime DateReported { get; set; }
        public string SaleType { get; set; }
        public double TotalSales { get; set;}
    }
}
