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
    public interface ICustomerService
    {
        Task<ResponseMessage> GetAllCustomer(RequestMessage requestMessage);
        Task<ResponseMessage> SaveCustomer(RequestMessage requestMessage);
        Task<ResponseMessage> GetCustomerById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteCustomer(RequestMessage requestMessage);

    }
}
