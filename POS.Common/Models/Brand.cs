using POS.Common.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
    public class Brand: BaseClass
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }

        [NotMapped]
        public virtual VMAttachment? LogoAttachment { get; set; }
    }
}
