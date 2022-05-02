using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.OrderDtos
{
    public class GetOrderNotificationNumber
    {
        public int OrderNotifNumberId { get; set; }
        public int BranchId { get; set; }
        public int HardwareStoreId { get; set; }
        public int NumberOfOrderNotif { get; set; }
    }
}
