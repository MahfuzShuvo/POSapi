using POS.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.Interface
{
    public interface IRolePermissionMappingService
    {
        Task<ResponseMessage> GetAllRolePermissionMapping(RequestMessage requestMessage);
        Task<ResponseMessage> GetRolePermissionMappingById(RequestMessage requestMessage);
        Task<ResponseMessage> SaveRolePermissionMapping(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteRolePermissionMapping(RequestMessage requestMessage);
    }
}
