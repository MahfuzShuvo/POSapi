using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpPost("GetDashboardInitialData")]
        public async Task<ResponseMessage> GetDashboardInitialData(RequestMessage requestMessage)
        {
            return await _dashboardService.GetDashboardInitialData(requestMessage);
        }

        
    }
}
