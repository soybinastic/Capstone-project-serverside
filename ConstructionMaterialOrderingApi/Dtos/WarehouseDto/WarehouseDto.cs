using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.WarehouseDto
{
    public class WarehouseDto
    {
        [Required]
        public string WarehouseName { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
