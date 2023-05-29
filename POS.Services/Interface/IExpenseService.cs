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
    public interface IExpenseService
    {
        Task<ResponseMessage> GetAllExpense(RequestMessage requestMessage);
        Task<ResponseMessage> SaveExpense(RequestMessage requestMessage);
        Task<ResponseMessage> GetExpenseById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteExpense(RequestMessage requestMessage);

    }
}
