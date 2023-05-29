using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Sales
    {
        public int SalesID { get; set; }
        public int? ProductID { get; set; }
        public int? Qty { get; set; }
        public double? Price { get; set; }
        public string SalesCode { get; set; }
    }
}
