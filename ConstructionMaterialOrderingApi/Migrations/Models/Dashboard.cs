using System;
using System.ComponentModel.DataAnnotations;

namespace ConstructionMaterialOrderingApi.Models
{
    public class Dashboard
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public double SalesOfMonth { get; set; }
        public double OriginalSales { get; set; }
        public double Profit { get; set; }
        public double PlatformFee { get; set; }
        public double Total { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
    }
}