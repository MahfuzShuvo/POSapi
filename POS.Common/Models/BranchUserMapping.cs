using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class BranchUserMapping
    {
        public int BranchUserMappingID { get; set; }
        public int? BranchID { get; set; }
        public int? SystemUserID { get; set; }
        public bool? IsManager { get; set; } = false;
    }
}
