using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.HardwareStoreUserDto
{
    public class UserInformationDto
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public int HardwareStoreId { get; set; }
        public string Name { get; set; }
        public int BranchId { get; set; }
        public int WarehouseId { get; set; }
        public string Role { get; set; }
    }
}
