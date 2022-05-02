using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class NotificationNumber
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int Number { get; set; }
    }
}
