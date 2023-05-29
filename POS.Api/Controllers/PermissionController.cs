using Microsoft.AspNetCore.Mvc;
using POS.Common.DTO;
using POS.Services.Interface;

namespace POS.Api.Controllers
{
    [Route("api/Permission")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpPost("GetAllPermission")]
        public async Task<ResponseMessage> GetAllPermission(RequestMessage requestMessage)
        {
            return await _permissionService.GetAllPermission(requestMessage);
        }

        [HttpPost("GetPermissionById")]
        public async Task<ResponseMessage> GetPermissionById(RequestMessage requestMessage)
        {
            return await _permissionService.GetPermissionById(requestMessage);
        }

        [HttpPost("SavePermission")]
        public async Task<ResponseMessage> SavePermissions(RequestMessage requestMessage)
        {
            return await _permissionService.SavePermission(requestMessage);
        }

        [HttpPost("DeletePermission")]
        public async Task<ResponseMessage> DeletePermission(RequestMessage requestMessage)
        {
            return await _permissionService.DeletePermission(requestMessage);
        }
    }
}
