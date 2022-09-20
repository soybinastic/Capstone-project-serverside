using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public int HardwareStoreId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateRegistered { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Range { get; set; }
        public string Image { get; set; }
    }
}
