using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class RolePermissionMapping
    {
        public int RolePermissionMappingID { get; set; }
        public int? RoleID { get; set; }
        public int? PermissionID { get; set; }
        [NotMapped]
        public bool IsChecked { get; set; }
    }
}
