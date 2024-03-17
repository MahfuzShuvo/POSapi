using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Coupon")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _CouponService;

        public CouponController(ICouponService CouponService)
        {
            _CouponService = CouponService;
        }

        [HttpPost("GetAllCoupon")]
        public async Task<ResponseMessage> GetAllCoupon(RequestMessage requestMessage)
        {
            return await _CouponService.GetAllCoupon(requestMessage);
        }

        [HttpPost("GetCouponById")]
        public async Task<ResponseMessage> GetCouponById(RequestMessage requestMessage)
        {
            return await _CouponService.GetCouponById(requestMessage);
        }

        [HttpPost("SaveCoupon")]
        public async Task<ResponseMessage> SaveCoupons(RequestMessage requestMessage)
        {
            return await _CouponService.SaveCoupon(requestMessage);
        }

        [HttpPost("DeleteCoupon")]
        public async Task<ResponseMessage> DeleteCoupon(RequestMessage requestMessage)
        {
            return await _CouponService.DeleteCoupon(requestMessage);
        }
    }
}
