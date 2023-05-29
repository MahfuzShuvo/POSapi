using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Category: BaseClass
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public bool? IsParent { get; set; }
    }
}
