using Microsoft.AspNetCore.Mvc;
using POS.Common.DTO;
using POS.Services.Interface;

namespace POS.Api.Controllers
{
    [Route("api/Role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("GetAllRole")]
        public async Task<ResponseMessage> GetAllRole(RequestMessage requestMessage)
        {
            return await _roleService.GetAllRole(requestMessage);
        }

        [HttpPost("GetRoleById")]
        public async Task<ResponseMessage> GetRoleById(RequestMessage requestMessage)
        {
            return await _roleService.GetRoleById(requestMessage);
        }

        [HttpPost("SaveRole")]
        public async Task<ResponseMessage> SaveRoles(RequestMessage requestMessage)
        {
            return await _roleService.SaveRole(requestMessage);
        }

        [HttpPost("DeleteRole")]
        public async Task<ResponseMessage> DeleteRole(RequestMessage requestMessage)
        {
            return await _roleService.DeleteRole(requestMessage);
        }
    }
}
