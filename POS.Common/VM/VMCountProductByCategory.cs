using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.VM
{
    public class VMCountProductByCategory
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? ProductCount { get; set; }
    }
}
