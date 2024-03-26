using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.DTO
{
    public class ExportCSV
    {
        public int BranchID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
