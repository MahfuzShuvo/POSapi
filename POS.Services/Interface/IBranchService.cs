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
    public interface IBranchService
    {
        Task<ResponseMessage> GetAllBranch(RequestMessage requestMessage);
        Task<ResponseMessage> SaveBranch(RequestMessage requestMessage);
        Task<ResponseMessage> GetBranchById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteBranch(RequestMessage requestMessage);
        Task<ResponseMessage> AssignUserToBranch(RequestMessage requestMessage);
        Task<ResponseMessage> RemoveUserFromBranch(RequestMessage requestMessage);
    }
}
