using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class OrderNotificationNumber
    {
        public int Id { get; set; }
        public int HardwareStoreId { get; set; }
        public int NumberOfOrder { get; set; }
    }
}
