using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructionMaterialOrderingApi.Models
{
    public class BestSellingReport
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public string BestSellingReportType { get; set; }
        public DateTime DateReported { get; set; }
        [ForeignKey("BestSellingReportId")]
        public List<BestSellingDetail> BestSellingDetails { get; set; }
    }
}
