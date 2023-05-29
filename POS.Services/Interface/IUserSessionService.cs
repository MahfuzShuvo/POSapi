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
    public interface IUserSessionService
    {
        Task<ResponseMessage> GetAllUserSession(RequestMessage requestMessage);
        Task<ResponseMessage> SaveUserSession(RequestMessage requestMessage);
        Task<ResponseMessage> GetUserSessionById(RequestMessage requestMessage);
        Task<ResponseMessage> GetUserSessionBySystemUserId(RequestMessage requestMessage);
    }
}
