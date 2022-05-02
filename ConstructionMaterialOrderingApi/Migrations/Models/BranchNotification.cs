using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class BranchNotification
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public DateTime DateNotified { get; set; }
    }
}
