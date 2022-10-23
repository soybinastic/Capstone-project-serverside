using System.ComponentModel.DataAnnotations;

namespace ConstructionMaterialOrderingApi.Dtos.EmployeeDtos
{
    public class CreateEmployee
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required, MinLength(5)]
        public string Password { get; set; }
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Role { get; set; }
    }
}