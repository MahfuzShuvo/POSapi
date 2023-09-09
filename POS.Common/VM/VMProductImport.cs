using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.VM
{
    public class VMProductImport
    {
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public int StockQuantity { get; set; }
        public int AlertQuantity { get; set; }
        public double PurchasePrice { get; set; }
        public double SellingPrice { get; set; }
        public string? Unit { get; set; }
        public string? ExpireDate { get; set; }
        public double Discount { get; set; } = 0;
        public int Tax { get; set; } = 0;
        public int ProfitMargin { get; set; } = 0;
    }
}
