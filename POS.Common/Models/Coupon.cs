using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Coupon:BaseClass
    {
        public int CouponID { get; set; }
        public string? CouponCode { get; set; }
        public int? Type { get; set; }
        public double? Value { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}
        [NotMapped]
        public List<string>? CouponDuration { get; set; }
    }
}
