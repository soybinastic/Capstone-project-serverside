using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.RecieveProductDtos
{
    public class RecieveProductDto
    {
        [Required]
        public int HardwareProductId { get; set; }
        [Required]
        public decimal CostPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
