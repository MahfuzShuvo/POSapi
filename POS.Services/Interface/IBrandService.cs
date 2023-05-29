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
    public interface IBrandService
    {
        Task<ResponseMessage> GetAllBrand(RequestMessage requestMessage);
        Task<ResponseMessage> SaveBrand(RequestMessage requestMessage);
        Task<ResponseMessage> GetBrandById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteBrand(RequestMessage requestMessage);

    }
}
