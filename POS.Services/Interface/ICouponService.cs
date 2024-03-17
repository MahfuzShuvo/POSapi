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
    public interface ICouponService
    {
        Task<ResponseMessage> GetAllCoupon(RequestMessage requestMessage);
        Task<ResponseMessage> SaveCoupon(RequestMessage requestMessage);
        Task<ResponseMessage> GetCouponById(RequestMessage requestMessage);
        Task<ResponseMessage> DeleteCoupon(RequestMessage requestMessage);

    }
}
