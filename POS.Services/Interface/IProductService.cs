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
    public interface IProductService
    {
        Task<ResponseMessage> GetAllProduct(RequestMessage requestMessage);
        Task<ResponseMessage> GetProductCountByCategory(RequestMessage requestMessage);
        Task<ResponseMessage> GetAllProductByCategoryID(RequestMessage requestMessage);
        Task<ResponseMessage> SaveProduct(RequestMessage requestMessage);
        Task<ResponseMessage> GetProductById(RequestMessage requestMessage);
        Task<ResponseMessage> GetProductBySlug(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteProduct(RequestMessage requestMessage);
        Task<ResponseMessage> GetInitialDataForSaveProduct(RequestMessage requestMessage);
        Task<ResponseMessage> ChangeProductStatus(RequestMessage requestMessage);
        Task<ResponseMessage> SearchProduct(RequestMessage requestMessage);
    }
}
