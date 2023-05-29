using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.VM
{
    public class VMPurchase
    {
        public string PurchaseCode { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public double? SubTotal { get; set; }
        public double? TotalPurchasePrice { get; set; }
        public double? OtherCharge { get; set; }
        public string DiscountType { get; set; }
        public double? Discount { get; set; }
        public string AccountTitle { get; set; }
        public double? PaymentAmount { get; set; }
        public double? DueAmount { get; set; }
        public string PaymentNote { get; set; }
        public string PurchaseStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string SupplierName { get; set; }
        public string CreatedByName { get; set; }
    }
}
