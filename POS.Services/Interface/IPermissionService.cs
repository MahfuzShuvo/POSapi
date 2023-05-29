using POS.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.Interface
{
    public interface IPermissionService
    {
        Task<ResponseMessage> GetAllPermission(RequestMessage requestMessage);
        Task<ResponseMessage> GetPermissionById(RequestMessage requestMessage);
        Task<ResponseMessage> SavePermission(RequestMessage requestMessage);
        Task<ResponseMessage> DeletePermission(RequestMessage requestMessage);
    }
}
