using POS.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.Interface
{
    /// <summary>
    /// Interface
    /// </summary>
    public interface ISystemUserService
    {
        Task<ResponseMessage> GetAllSystemUser(RequestMessage requestMessage);
        Task<ResponseMessage> SaveSystemUser(RequestMessage requestMessage);
        Task<ResponseMessage> GetSystemUserById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteSystemUser(RequestMessage requestMessage);

    }
}
