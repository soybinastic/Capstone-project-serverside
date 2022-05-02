using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class CustomerVerification
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public string NbiImageFile { get; set; }
        public string BarangayClearanceImageFile { get; set; }
        public string GovernmentIdImageFile { get; set; }
        public string BankStatementImageFile { get; set; }
        public DateTime DateSubmitted { get; set; }
    }
}
