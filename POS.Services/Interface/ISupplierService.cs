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
    public interface ISupplierService
    {
        Task<ResponseMessage> GetAllSupplier(RequestMessage requestMessage);
        Task<ResponseMessage> SaveSupplier(RequestMessage requestMessage);
        Task<ResponseMessage> GetSupplierById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteSupplier(RequestMessage requestMessage);

    }
}
