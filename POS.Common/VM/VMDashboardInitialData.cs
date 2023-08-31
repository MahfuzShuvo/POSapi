using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.VM
{
    public class VMDashboardInitialData
    {
        public int DayNumber { get; set; }
        public double? TotalSales { get; set; }
        public double? TotalPurchases { get; set; }
        public double? TotalExpenses { get; set; }
    }
}
