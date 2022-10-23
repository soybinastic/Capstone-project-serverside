using System;

namespace ConstructionMaterialOrderingApi.Models
{
    public class VerifiedUser
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTime DateConfirmed { get; set; } = DateTime.UtcNow;
    }
}