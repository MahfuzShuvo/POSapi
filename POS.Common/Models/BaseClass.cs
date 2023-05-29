using POS.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Models
{
  public class BaseClass
  {
    public int CreatedBy { get; set; } = 0;

    public DateTime? CreatedDate { get; set; }

    public int UpdatedBy { get; set; } = 0;

    public DateTime? UpdatedDate { get; set; } = CommonConstant.DeafultDate;

    public int Status { get; set; } = 1;
  }
}
