using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class HardwareStoreUser
    {
        public int Id { get; set; }
        public string ApplicationUserAccountId { get; set; }
        public int HardwareStoreId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserFrom { get; set; }
        public string Role { get; set; }
    }
}
