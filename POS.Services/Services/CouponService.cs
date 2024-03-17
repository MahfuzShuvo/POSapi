using POS.Common.Constants;
using POS.Common.DTO;
using POS.Common.Enums;
using POS.Common.Helper;
using POS.Common.Models;
using POS.DataAccess;
using POS.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services
{
    public class CouponService: ICouponService
    {
        private readonly POSDbContext _posDbContext;

        public CouponService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Coupon
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllCoupon(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Coupon> lstCoupon = new List<Coupon>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstCoupon = await _posDbContext.Coupon.OrderBy(x => x.CouponID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstCoupon.Count;


                responseMessage.ResponseObj = lstCoupon;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllCoupon");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllCoupon");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetCouponById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Coupon objCoupon = new Coupon();
                int CouponID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objCoupon = await _posDbContext.Coupon.FirstOrDefaultAsync(x => x.CouponID == CouponID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objCoupon;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetCouponById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetCouponById");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> DeleteCoupon(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Coupon objCoupon = new Coupon();
                int CouponID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objCoupon = await _posDbContext.Coupon.AsNoTracking().FirstOrDefaultAsync(x => x.CouponID == CouponID);
               
                if (objCoupon.CouponID > 0)
                {

                    _posDbContext.Coupon.Remove(objCoupon);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Coupon not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteCoupon");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteCoupon");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Coupon
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveCoupon(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Coupon objCoupon = JsonConvert.DeserializeObject<Coupon>(requestMessage?.RequestObj.ToString());
                if (objCoupon != null)
                {
                    if (CheckedValidation(objCoupon, responseMessage))
                    {
                        if (objCoupon.CouponDuration?.Count > 0)
                        {
                            objCoupon.StartDate = Convert.ToDateTime(objCoupon.CouponDuration[0]);
                            objCoupon.EndDate = Convert.ToDateTime(objCoupon.CouponDuration[1]);
                        }
                        if (objCoupon.CouponID > 0)
                        {
                            Coupon existingCoupon = await this._posDbContext.Coupon.AsNoTracking().FirstOrDefaultAsync(x => x.CouponID == objCoupon.CouponID);
                            if (existingCoupon != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objCoupon.CreatedDate = existingCoupon.CreatedDate;
                                objCoupon.CreatedBy = existingCoupon.CreatedBy;
                                objCoupon.UpdatedDate = DateTime.Now;
                                objCoupon.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Coupon.Update(objCoupon);

                            }
                        }
                        else
                        {
                            //objCoupon.Status = (int)Enums.Status.Active;
                            objCoupon.CreatedDate = DateTime.Now;
                            objCoupon.CreatedBy = requestMessage.UserID;
                            await _posDbContext.Coupon.AddAsync(objCoupon);

                        }

                     
                        await _posDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = objCoupon;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveCoupon");
                    }
                    else
                    {
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                    }
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.SaveFailed;
                }

            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveCoupon");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objCoupon"></param>
        /// <returns></returns>
        private bool CheckedValidation(Coupon objCoupon, ResponseMessage responseMessage)
        {

            bool result = true;
            Coupon existingCoupon = new Coupon();


            if (String.IsNullOrEmpty(objCoupon.CouponCode))
            {
                responseMessage.Message = "Coupon code is required";
                return false;
            }
            
            return true;
        }
    }
}
