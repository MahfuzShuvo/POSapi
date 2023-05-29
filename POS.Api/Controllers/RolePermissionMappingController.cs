using Microsoft.AspNetCore.Mvc;
using POS.Common.DTO;
using POS.Services.Interface;

namespace POS.Api.Controllers
{
    [Route("api/RolePermissionMapping")]
    [ApiController]
    public class RolePermissionMappingController : ControllerBase
    {
        private readonly IRolePermissionMappingService _rolePermissionMappingService;

        public RolePermissionMappingController(IRolePermissionMappingService rolePermissionMappingService)
        {
            _rolePermissionMappingService = rolePermissionMappingService;
        }

        [HttpPost("GetAllRolePermissionMapping")]
        public async Task<ResponseMessage> GetAllRolePermissionMapping(RequestMessage requestMessage)
        {
            return await _rolePermissionMappingService.GetAllRolePermissionMapping(requestMessage);
        }

        [HttpPost("GetRolePermissionMappingById")]
        public async Task<ResponseMessage> GetRolePermissionMappingById(RequestMessage requestMessage)
        {
            return await _rolePermissionMappingService.GetRolePermissionMappingById(requestMessage);
        }

        [HttpPost("SaveRolePermissionMapping")]
        public async Task<ResponseMessage> SaveRolePermissionMappings(RequestMessage requestMessage)
        {
            return await _rolePermissionMappingService.SaveRolePermissionMapping(requestMessage);
        }

        [HttpPost("DeleteRolePermissionMapping")]
        public async Task<ResponseMessage> DeleteRolePermissionMapping(RequestMessage requestMessage)
        {
            return await _rolePermissionMappingService.DeleteRolePermissionMapping(requestMessage);
        }
    }
}
