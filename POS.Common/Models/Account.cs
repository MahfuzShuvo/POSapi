using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public string AccountTitle { get; set; }
        public string? Description { get; set; }
        [NotMapped]
        public double Balance { get; set; } = 0;
        public string? AccountNumber { get; set; }
        public int? Status { get; set; }
        [NotMapped]
        public int BranchID { get; set; }
    }
}
