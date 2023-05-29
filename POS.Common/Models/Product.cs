using POS.Common.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Product: BaseClass
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public string? SKU { get; set; }
        public string? Slug { get; set; }
        public string Image { get; set; }
        public int Unit { get; set; } = 0;
        [NotMapped]
        public string UnitName { get; set; }
        public int CategoryID { get; set; } = 0;
        public int BrandID { get; set; } = 0;
        public DateTime? ExpireDate { get; set; }
        public int Qty { get; set; } = 1;
        public int MinQty { get; set; } = 0;
        public double Cost { get; set; } = 0;
        public double Price { get; set; } = 0;
        public double PurchasePrice { get; set; } = 0;
        public double SellingPrice { get; set; } = 0;
        public double FinalPrice { get; set; } = 0;
        public int ProfitMargin { get; set; } = 0;
        public int TaxType { get; set; } = 0;
        public int Tax { get; set; } = 0;
        public double Discount { get; set; } = 0;
        public int DiscountType { get; set; } = 0;

        [NotMapped]
        public virtual VMAttachment? Attachment { get; set; }
    }
}
