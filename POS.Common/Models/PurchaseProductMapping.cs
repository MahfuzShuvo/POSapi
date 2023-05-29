using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class PurchaseProductMapping
    {
        public int PurchaseProductMappingID { get; set; }
        public int? PurchaseID { get; set; }
        public int? ProductID { get; set; }
        public double? UnitPrice { get; set; }
        public int? Qty { get; set; }
        public double? TotalPrice { get; set; }
    }
}
