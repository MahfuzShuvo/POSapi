using POS.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.Interface
{
    public interface ISecurityService
    {
        Task<ResponseMessage> Login(RequestMessage requestMessage);
        Task<ResponseMessage> Logout(RequestMessage requestMessage);
        Task<ResponseMessage> Register(RequestMessage requestMessage);
        Task<bool> CheckPermission(string url);
    }
}
