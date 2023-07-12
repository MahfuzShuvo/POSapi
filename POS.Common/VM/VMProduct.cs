using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.VM
{
    public class VMProduct
    {
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public string SKU { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public double? PurchasePrice { get; set; }
        public double? FinalPrice { get; set; }
        public int? Qty { get; set; }
        public int? MinQty { get; set; }
        public int? Status { get; set; }
        public int? CategoryID { get; set; }
        public int? BrandID { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string UnitName { get; set; }
    }
}
