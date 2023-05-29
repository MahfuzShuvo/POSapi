using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/SystemUser")]
    [ApiController]
    public class SystemUserController : ControllerBase
    {
        private readonly ISystemUserService _userService;

        public SystemUserController(ISystemUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("GetAllSystemUser")]
        public async Task<ResponseMessage> GetAllSystemUser(RequestMessage requestMessage)
        {
            return await _userService.GetAllSystemUser(requestMessage);
        }

        [HttpPost("GetSystemUserById")]
        public async Task<ResponseMessage> GetSystemUserById(RequestMessage requestMessage)
        {
            return await _userService.GetSystemUserById(requestMessage);
        }

        [HttpPost("SaveSystemUser")]
        public async Task<ResponseMessage> SaveSystemUsers(RequestMessage requestMessage)
        {
            return await _userService.SaveSystemUser(requestMessage);
        }

        [HttpPost("DeleteSystemUser")]
        public async Task<ResponseMessage> DeleteSystemUser(RequestMessage requestMessage)
        {
            return await _userService.DeleteSystemUser(requestMessage);
        }
    }
}
