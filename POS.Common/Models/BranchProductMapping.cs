using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class BranchProductMapping
    {
        public int BranchProductMappingID { get; set; }
        public int? BranchID { get; set; }
        public int? ProductID { get; set; }
        public int? Quantity { get; set; } = 0;
    }
}
