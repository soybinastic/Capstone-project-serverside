using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ConstructionMaterialOrderingApi.Dtos.BranchDto
{
    public class AddBranchDto
    {
        [Required]
        public string BranchName { get; set; }
        [Required]
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public IFormFile Image { get; set; }
    }
}
