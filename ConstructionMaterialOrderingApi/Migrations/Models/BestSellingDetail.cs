using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructionMaterialOrderingApi.Models
{
    public class BestSellingDetail
    {
        public int Id { get; set; }
        public int BestSellingReportId { get; set; }
        public DateTime BestSellingDate { get; set; }
        [ForeignKey("BestSellingDetailId")]
        public List<BestSellingProductReport> BestSellingProductReports { get; set; }
    }
}
