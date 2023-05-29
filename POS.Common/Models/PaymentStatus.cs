using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class PaymentStatus
    {
        public int PaymentStatusID { get; set; }
        public double? PaidAmount { get; set; }
        public double? DueAmount { get; set; }
        public double? TotalAmount { get; set; }
    }
}
