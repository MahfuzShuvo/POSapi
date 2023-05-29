using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/UserSession")]
    [ApiController]
    public class UserSessionController : ControllerBase
    {
        private readonly IUserSessionService _userSessionService;
        public UserSessionController(IUserSessionService userService)
        {
            _userSessionService = userService;
        }

        [HttpPost("GetAllUserSession")]
        public async Task<ResponseMessage> GetAllUserSession(RequestMessage requestMessage)
        {
            return await _userSessionService.GetAllUserSession(requestMessage);
        }

        [HttpPost("GeUserSessionById")]
        public async Task<ResponseMessage> GetUserSessionById(RequestMessage requestMessage)
        {
            return await _userSessionService.GetUserSessionById(requestMessage);
        }

        [HttpPost("SaveUserSession")]
        public async Task<ResponseMessage> SaveUserSession(RequestMessage requestMessage)
        {
            return await _userSessionService.SaveUserSession(requestMessage);
        }
    }
}
