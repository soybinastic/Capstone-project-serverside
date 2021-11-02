using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.AdminDtos
{
    public class RegisterHardwareStoreDto
    {
        [Required(ErrorMessage = "Firstname of owner is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Lastname of owner is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Username of this store is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Name of store is required.")]
        public string HardwareStoreName { get; set; }
        [Required(ErrorMessage = "Name of owner is required.")]
        public string Owner { get; set; }
        [Required(ErrorMessage = "Business address is required.")]
        public string BusinessAddress { get; set; }
        [Required(ErrorMessage = "Contact number is required.")]
        public string ContactNo { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } 
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
