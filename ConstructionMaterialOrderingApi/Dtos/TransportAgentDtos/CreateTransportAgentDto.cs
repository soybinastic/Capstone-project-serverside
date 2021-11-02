using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.TransportAgentDtos
{
    public class CreateTransportAgentDto
    {
        [Required(ErrorMessage = "Firstname of transport agent is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Lastname of transport agent is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } 
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
