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
    public interface IPurchaseService
    {
        Task<ResponseMessage> GetAllPurchase(RequestMessage requestMessage);
        Task<ResponseMessage> SavePurchase(RequestMessage requestMessage);
        Task<ResponseMessage> GetPurchaseById(RequestMessage requestMessage);
        Task<ResponseMessage> GetPurchaseByPurchaseCode(RequestMessage requestMessage);
        Task<ResponseMessage> DeletePurchase(RequestMessage requestMessage);

    }
}
