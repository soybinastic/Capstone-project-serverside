using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class Request
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public DateTime DateRequest { get; set; }
        [ForeignKey("RequestId")]
        public List<RequestProduct> RequestProducts { get; set; }
    }
}
