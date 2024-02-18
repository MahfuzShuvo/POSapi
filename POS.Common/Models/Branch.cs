using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Branch:BaseClass
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string? Address { get; set; }
        public int? BranchManagerID { get; set; }
    }
}
