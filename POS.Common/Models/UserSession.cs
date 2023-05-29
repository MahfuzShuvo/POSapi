using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class UserSession
    {
        public int UserSessionID { get; set; }

        public string? Token { get; set; }

        public int? SystemUserID { get; set; }

        public DateTime? SessionStart { get; set; }

        public DateTime? SessionEnd { get; set; }

        public int? RoleID { get; set; } = 0;

        public int? Status { get; set; } = 0;

        public int? CreatedBy { get; set; } = 0;

        public DateTime? CreatedDate { get; set; }

    }
}
