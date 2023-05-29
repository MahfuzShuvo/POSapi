using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.VM
{
    public class VMAccountStatement
    {
        public DateTime? TransactionDate { get; set; }
        public double InBalance { get; set; } = 0;
        public double OutBalance { get; set; } = 0;
        public double Balance { get; set; } = 0;
    }
}
