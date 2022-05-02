using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class RecipientItem
    {
        public int Id { get; set; }
        public int RecipientId { get; set; }
        public Recipient Recipient { get; set; }
        public int HardwareProductId { get; set; }
        public HardwareProduct HardwareProduct { get; set; }
        public double Amount { get; set; }
    }
}
