using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Expense: BaseClass
    {
        public int ExpenseID { get; set; }
        public string? ExpenseTitle { get; set; }
        public string? Description { get; set; }
        public double Amount { get; set; }
        public int PurchaseID { get; set; } = 0;
        public int AccountID { get; set; }
    }

}
