using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.ProductDtos
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Category of product is required.")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Name of product is required")]
        public string Name { get; set; } 
        
        public string Description { get; set; }
        [Required(ErrorMessage = "Brand  of product is required.")]
        public string Brand { get; set; }
        [Required(ErrorMessage = "Quality  of product is required.")]
        public string Quality { get; set; }
        [Required(ErrorMessage = "Price  of product is required.")]
        public double Price { get; set; }
        [Required(ErrorMessage = "Stocks  of product is required.")]
        public int StockNumber { get; set; }
    }
}
