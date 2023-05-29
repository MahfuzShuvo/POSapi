using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.VM
{
    public class VMGetAccountBalanceExpense
    {
        public int AccountID { get; set; }
        public string? AccountTitle { get; set; }
        public double? Balance { get; set; }
        public double? Expense { get; set; }
        public double? CurrentBalance { get; set; }
        public string? AccountNumber { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }

    }
}
