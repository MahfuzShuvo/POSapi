using POS.Common.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Purchase: BaseClass
    {
        public int PurchaseID { get; set; }
        public string? PurchaseCode { get; set; }
        public DateTime? PurchaseDate { get; set; } = DateTime.UtcNow;
        public int? PurchaseStatus { get; set; }
        public int? SupplierID { get; set; }
        public double? SubTotal { get; set; }
        public double? TotalPurchasePrice { get; set; }
        public double? OtherCharge { get; set; }
        public int? DiscountType { get; set; }
        public double? Discount { get; set; }
        public int? PaymentType { get; set; }
        public double? PaymentAmount { get; set; }
        public double? DueAmount { get; set; }
        public string PaymentNote { get; set; }
        [NotMapped]
        public List<VMProduct> lstProduct { get; set; } = new List<VMProduct>();
    }
}
