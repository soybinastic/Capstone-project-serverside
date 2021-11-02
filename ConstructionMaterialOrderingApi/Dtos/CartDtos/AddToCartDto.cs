using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.CartDtos
{
    public class AddToCartDto
    {
        [Required]
        public int HardwareStoreId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public double ProductPrice { get; set; }
        [Required]
        public string ProductDescription { get; set; }
        [Required]
        public string ProductBrand { get; set; }
        [Required]
        public string ProductQuality { get; set; }
    }
}
