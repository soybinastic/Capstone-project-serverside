using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int HardwareProductId { get; set; }
        public int HardwareStoreId { get; set; }
        public int BranchId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Quality { get; set; }
        public double Price { get; set; }
        public int StockNumber { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsAvailableInWarehouse { get; set; }
        public string ImageFile { get; set; }
    }
}
