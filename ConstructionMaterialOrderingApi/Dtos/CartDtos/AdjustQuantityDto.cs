using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.CartDtos
{
    public class AdjustQuantityDto
    {
        [Required]
        public int Quantity { get; set; }
    }
}
