using System;

namespace ConstructionMaterialOrderingApi.Models
{
    public class RegisteredCompany
    {
        public int Id { get; set; }
        public int HardwareStoreId { get; set; }
        public HardwareStore HardwareStore { get; set; }
        public string RegisteredBy { get; set; }
        public DateTime DateConfirmed { get; set; } = DateTime.UtcNow;
    }
}