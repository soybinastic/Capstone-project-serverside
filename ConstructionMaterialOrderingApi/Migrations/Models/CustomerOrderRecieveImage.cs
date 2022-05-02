using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class CustomerOrderRecieveImage
    {
        public int Id { get; set; }
        public int BranchId { get; set;}
        public int OrderId { get; set; }
        public string ImageFile { get; set; }
    }
}
