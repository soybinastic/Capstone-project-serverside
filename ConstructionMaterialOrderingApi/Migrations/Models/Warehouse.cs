using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public int HardwareStoreId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
