using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.BranchDto
{
    public class UpdateBranchDto
    {
        public int Id { get; set; }
        [Required]
        public string BranchName { get; set; }
        [Required]
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
