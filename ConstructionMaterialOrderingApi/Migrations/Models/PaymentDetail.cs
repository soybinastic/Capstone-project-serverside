using System;

namespace ConstructionMaterialOrderingApi.Models
{
    public class PaymentDetail
    {
        public int Id { get; set; }
        public int DashboardId { get; set; }
        public Dashboard Dashboard { get; set; }
        public DateTime PaidAt { get; set; }
        public string PaymentGateway { get; set; }
        public double TotalAmount { get; set; }
    }
}