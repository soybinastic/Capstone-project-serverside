using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class CustomerOrderDetails
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        //public string NBIImageFile { get; set; }
        //public string BarangayClearanceImageFile { get; set; }
        //public string GovernmentIdImageFile { get; set; }
        //public string BankStatementImageFile { get; set; }
        //public int Age { get; set; }
        //public DateTime BirthDate { get; set; }
    }
}
