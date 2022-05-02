using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.CustomerDtos
{
    public class GetCustomerDto
    {
        public int CustomerId { get; set; }
        public string AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsVerified { get; set; }
    }
}
