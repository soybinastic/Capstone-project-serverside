using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.CustomerVerificationDtos
{
    public class VerificationDetail
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public IFormFile Nbi { get; set; }
        [Required]
        public IFormFile BarangayClearance { get; set; }
        [Required]
        public IFormFile GovernmentId { get; set; }
        public IFormFile BankStatement { get; set; }
    }
}
