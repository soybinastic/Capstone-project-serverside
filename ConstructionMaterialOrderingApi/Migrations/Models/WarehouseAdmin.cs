using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class WarehouseAdmin
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public int HardwareStoreId { get; set; }
        public int WarehouseId { get; set; }
        public string Name { get; set; }
    }
}
