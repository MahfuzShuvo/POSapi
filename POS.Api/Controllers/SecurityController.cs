using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using POS.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {

        private readonly ISecurityService _securityService;
        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        [HttpPost("Login")]
        public async Task<ResponseMessage> Login(RequestMessage requestMessage)
        {
            return await _securityService.Login(requestMessage);
        }

        [HttpPost("Register")]
        public async Task<ResponseMessage> Register(RequestMessage requestMessage)
        {
            return await _securityService.Register(requestMessage);
        }

        [HttpPost("Logout")]
        public async Task<ResponseMessage> Logout(RequestMessage requestMessage)
        {
            return await _securityService.Logout(requestMessage);
        }
    }
}
