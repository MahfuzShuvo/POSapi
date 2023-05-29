using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Permission: BaseClass
    {
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public string DisplayName { get; set; }
    }
}
