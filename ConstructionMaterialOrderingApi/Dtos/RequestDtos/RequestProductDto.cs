using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.RequestDtos
{
    public class RequestProductDto
    {
        [Required]
        public int HardwareProductId { get; set; }
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
