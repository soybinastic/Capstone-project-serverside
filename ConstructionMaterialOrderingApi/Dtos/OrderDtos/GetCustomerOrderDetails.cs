using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.OrderDtos
{
    public class GetCustomerOrderDetails
    {
        public int CustomerOrderDetailId { get; set; }
        public int OrderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public string Nbi { get; set; }
        public string BarangayClearance { get; set; }
        public string GovernmentId { get; set; }
        public string BankStatement { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
