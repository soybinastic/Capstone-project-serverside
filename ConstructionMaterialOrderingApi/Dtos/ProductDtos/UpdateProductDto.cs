using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.ProductDtos
{
    public class UpdateProductDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Quality { get; set; } 
        [Required]
        public double Price { get; set; } 
        [Required]
        public int StockNumber { get; set; }
        public bool IsAvailable { get; set; }
    }
}
