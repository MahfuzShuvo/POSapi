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
    public interface ICategoryService
    {
        Task<ResponseMessage> GetAllCategory(RequestMessage requestMessage);
        Task<ResponseMessage> SaveCategory(RequestMessage requestMessage);
        Task<ResponseMessage> GetCategoryById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteCategory(RequestMessage requestMessage);

    }
}
