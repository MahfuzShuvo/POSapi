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
    public interface IUnitService
    {
        Task<ResponseMessage> GetAllUnit(RequestMessage requestMessage);
        Task<ResponseMessage> SaveUnit(RequestMessage requestMessage);
        Task<ResponseMessage> GetUnitById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteUnit(RequestMessage requestMessage);

    }
}
