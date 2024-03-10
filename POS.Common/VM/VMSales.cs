using POS.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.VM
{
    public class VMSales
    {
        public string SalesCode { get; set; }
        public DateTime? SalesDate { get; set; }
        public int BranchID { get; set; }
        public double? SubTotal { get; set; }
        public double? TotalSalesPrice { get; set; }
        public string DiscountType { get; set; }
        public double? Discount { get; set; }
        public string? AccountTitle { get; set; }
        public double? PayAmount { get; set; }
        public double? DueAmount { get; set; }
        public string? SalesStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CustomerName { get; set; }
        public string CreatedByName { get; set; }
        public int? Status { get; set; }
        [NotMapped]
        public List<VMProduct> lstProduct { get; set; } = new List<VMProduct>();
        [NotMapped]
        public Customer objCustomer { get; set; } = new Customer();
    }
}
