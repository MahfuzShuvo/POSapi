using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class AccountStatement
    {
        public int AccountStatementID { get; set; }
        public int AccountID { get; set; } = 0;
        public int ExpenseID { get; set; } = 0;
        public double InBalance { get; set; } = 0;
        public double OutBalance { get; set; } = 0;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
