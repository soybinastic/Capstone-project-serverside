using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.CustomerDtos
{
    public class UpdateCustomerDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string ContactNo { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
    }
}
