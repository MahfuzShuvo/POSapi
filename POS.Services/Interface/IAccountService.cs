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
    public interface IAccountService
    {
        Task<ResponseMessage> GetAllAccount(RequestMessage requestMessage);
        Task<ResponseMessage> SaveAccount(RequestMessage requestMessage);
        Task<ResponseMessage> GetAccountById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteAccount(RequestMessage requestMessage);
        Task<ResponseMessage> AddBalance(RequestMessage requestMessage);
        Task<ResponseMessage> ShowAccountStatement(RequestMessage requestMessage);
    }
}
