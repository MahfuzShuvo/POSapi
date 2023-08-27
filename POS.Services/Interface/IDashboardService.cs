using POS.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.Interface
{
    /// <summary>
    /// Interface
    /// </summary>
    public interface IDashboardService
    {
        Task<ResponseMessage> GetDashboardInitialData(RequestMessage requestMessage);

    }
}
