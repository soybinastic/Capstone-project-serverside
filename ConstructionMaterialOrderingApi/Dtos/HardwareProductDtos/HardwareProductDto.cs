using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.HardwareProductDtos
{
    public class HardwareProductDto
    {
        public int HardwareStoreId { get; set; }
        public int WarehouseId { get; set; }
        [Required(ErrorMessage = "Item name is required")]
        public string ItemName { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Supplier is required")]
        public string Supplier { get; set; }
        public int StockNumber { get; set; }
        [Required(ErrorMessage = "Cost price is required")]
        public decimal CostPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
