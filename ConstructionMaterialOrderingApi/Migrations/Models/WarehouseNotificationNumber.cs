using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class WarehouseNotificationNumber
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int Number { get; set; }
    }
}
