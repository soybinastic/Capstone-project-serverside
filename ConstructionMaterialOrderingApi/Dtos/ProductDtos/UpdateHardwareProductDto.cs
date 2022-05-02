using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ConstructionMaterialOrderingApi.Dtos.ProductDtos
{
    public class UpdateHardwareProductDto
    {
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Quality { get; set; }
    }
}
