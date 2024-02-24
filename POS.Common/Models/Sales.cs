using POS.Common.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Sales: BaseClass
    {
        public int SalesID { get; set; }
        public string SalesCode { get; set; }
        public DateTime SalesDate { get; set; } = DateTime.UtcNow;
        public int SalesStatus { get; set; } = 1;
        public int BranchID { get; set; }
        public int? CustomerID { get; set; }
        public double? SubTotal { get; set; }
        public double? TotalSalesPrice { get; set; }
        public int? DiscountType { get; set; }
        public double? Discount { get; set; }
        public double? PayAmount { get; set; }
        public double? DueAmount { get; set; }
        public int? AccountID { get; set; }
        [NotMapped]
        public List<VMProduct> lstProduct { get; set; }= new List<VMProduct>();
        [NotMapped]
        public Customer objCustomer { get; set; } = new Customer();
    }
}
