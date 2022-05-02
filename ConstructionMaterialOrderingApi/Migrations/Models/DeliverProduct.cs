using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class DeliverProduct
    {
        public int Id { get; set; }
        public int WarehouseReportId { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public int WarehouseProductId { get; set; }
        public WarehouseProduct WarehouseProduct { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public int Quantity { get; set; }
    }
}
