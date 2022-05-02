using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.DepositSlipDtos
{
    public class DepositSlipDto
    {
        [Required]
        public string Depositor { get; set; }
        [Required]
        public DateTime DateDeposited { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public double AmountDeposited { get; set; }
    }
}
