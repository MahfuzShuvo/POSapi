using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Invoice: BaseClass
    {
        public int InvoiceID { get; set; }
        public int? CustomerID { get; set; }
        public string SalesCode { get; set; }
        public string InvoiceCode { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public int? PaymentType { get; set; }
        public int? PaymentStatusID { get; set; }
    }
}
