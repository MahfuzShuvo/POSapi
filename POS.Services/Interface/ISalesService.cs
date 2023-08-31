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
    public interface ISalesService
    {
        Task<ResponseMessage> GetAllSales(RequestMessage requestMessage);
        Task<ResponseMessage> SaveSales(RequestMessage requestMessage);
        Task<ResponseMessage> GetSalesById(RequestMessage requestMessage);
        Task<ResponseMessage> GetSalesBySalesCode(RequestMessage requestMessage);
        Task<ResponseMessage> GetSalesBySalesCodeForView(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteSales(RequestMessage requestMessage);

    }
}
