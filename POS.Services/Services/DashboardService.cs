using POS.Common.Constants;
using POS.Common.DTO;
using POS.Common.Enums;
using POS.Common.Helper;
using POS.Common.Models;
using POS.DataAccess;
using POS.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using POS.Common.VM;
using POS.Common.QueryHelper;

namespace POS.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly POSDbContext _posDbContext;
        private readonly IConfiguration _configuration;
        public DashboardService(POSDbContext ctx, IConfiguration configuration)
        {
            _posDbContext = ctx;
            this._configuration = configuration;
        }

        /// <summary>
        /// Get dashboard initial data
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetDashboardInitialData(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                DateTime currentDate = DateTime.Now;

                DashboardDTO objDashboard = JsonConvert.DeserializeObject<DashboardDTO>(requestMessage.RequestObj?.ToString());


                if (objDashboard?.MonthNumber == 0)
                {
                    objDashboard.MonthNumber = currentDate.Month;
                }

                List<VMDashboardInitialData> lstVMDashboardInitialData = new List<VMDashboardInitialData>();

                string sql = SQLContent.GetDashboardInitialDataQuery(objDashboard.MonthNumber, objDashboard.BranchID);
                lstVMDashboardInitialData = _posDbContext.VMDashboardInitialData.FromSqlRaw(sql).ToList();

                responseMessage.ResponseObj = lstVMDashboardInitialData;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetDashboardInitialData");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllDashboard");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

    }
}
