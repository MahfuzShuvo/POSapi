using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Helper.AuditLog
{
    public class ExceptionLog
    {
        public long ExceptionLogID { get; set; }
        public string InputObject { get; set; }
        public string ExceptionMessage { get; set; }
        public int UserId { get; set; } = 0;
        public int ActionTypeID { get; set; } = 0;
        public string MethodName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
