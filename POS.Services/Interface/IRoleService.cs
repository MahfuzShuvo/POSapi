using POS.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.Interface
{
    public interface IRoleService
    {
        Task<ResponseMessage> GetAllRole(RequestMessage requestMessage);
        Task<ResponseMessage> GetRoleById(RequestMessage requestMessage);
        Task<ResponseMessage> SaveRole(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteRole(RequestMessage requestMessage);
    }
}
