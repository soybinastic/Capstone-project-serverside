using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Models
{
    public class DepositSlip
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public string Depositor { get; set; }
        public DateTime DateDeposited { get; set; }
        public string BankName { get; set; }
        public double Amount { get; set; }
    }
}
