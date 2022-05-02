using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.MoveProductDto
{
    public class MoveProductDto
    {
        [Required]
        public int FromWarehouseId { get; set; }
        [Required]
        public int HardwareProductId { get; set; }
        [Required]
        public int MoveToWarehouseId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public DateTime MoveDate { get; set; }
    }
}
