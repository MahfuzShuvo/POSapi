using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.DTO
{
    public class RequestMessage
    {
        public object? RequestObj { get; set; }
        public int PageRecordSize { get; set; } = 0;
        public int PageNumber { get; set; } = 0;
        public int UserID { get; set; }

        public static implicit operator string(RequestMessage v)
        {
            throw new NotImplementedException();
        }
    }
}
