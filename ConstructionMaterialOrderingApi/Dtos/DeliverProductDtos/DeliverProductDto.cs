using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.DeliverProductDtos
{
    public class DeliverProductDto
    {
        public int RequestProductId { get; set; }
        [Required]
        public int HardwareProductId { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
