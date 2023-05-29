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
#pragma warning disable CS8600
    public class UserSessionService : IUserSessionService
    {
        private readonly POSDbContext _posDbContext;

        public UserSessionService(POSDbContext ctx)
        {
            this._posDbContext = ctx;
        }

        /// <summary>
        /// Get all System User
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllUserSession(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<UserSession> lstUserSession = new List<UserSession>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstUserSession = await _posDbContext.UserSession.OrderBy(x => x.UserSessionID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.ResponseObj = lstUserSession;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllUserSession");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllUserSession");
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
        public async Task<ResponseMessage> GetUserSessionById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                UserSession objUserSession = new UserSession();
                int UserSessionID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objUserSession = await _posDbContext.UserSession.FirstOrDefaultAsync(x => x.UserSessionID == UserSessionID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objUserSession;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetUserSessionById");
            }
            catch (Exception ex)
            {
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetUserSessionById");
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
        public async Task<ResponseMessage> GetUserSessionBySystemUserId(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                UserSession objUserSession = new UserSession();
                int systemUserId = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objUserSession = await _posDbContext.UserSession.AsNoTracking().OrderBy(u => u.UserSessionID).LastOrDefaultAsync(x => x.SystemUserID == systemUserId && x.Status == (int)Enums.Status.Active && x.SessionEnd > DateTime.Now);
                responseMessage.ResponseObj = objUserSession;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetUserSessionBySystemUserId");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetUserSessionBySystemUserId");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update System user
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveUserSession(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                UserSession objUserSession = JsonConvert.DeserializeObject<UserSession>(requestMessage?.RequestObj.ToString());

                if (objUserSession != null)
                {
                    if (CheckedValidation(objUserSession, responseMessage))
                    {
                        if (objUserSession.UserSessionID > 0)
                        {
                            UserSession existingUserSession = await this._posDbContext.UserSession.AsNoTracking().FirstOrDefaultAsync(x => x.UserSessionID == objUserSession.UserSessionID && x.Status == (int)Enums.Status.Active);
                            if (existingUserSession != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objUserSession.CreatedDate = existingUserSession.CreatedDate;
                                objUserSession.CreatedBy = existingUserSession.CreatedBy;
                                _posDbContext.UserSession.Update(objUserSession);

                            }
                        }
                        else
                        {
                            objUserSession.Status = (int)Enums.Status.Active;
                            objUserSession.CreatedDate = DateTime.Now;
                            objUserSession.CreatedBy = requestMessage.UserID;
                            await _posDbContext.UserSession.AddAsync(objUserSession);

                        }

                        await _posDbContext.SaveChangesAsync();
                        responseMessage.ResponseObj = objUserSession;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveUserSession");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveUserSession");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }


            return responseMessage;
        }
        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objUserSession"></param>
        /// <returns></returns>
        private bool CheckedValidation(UserSession objUserSession, ResponseMessage responseMessage)
        {
            if (string.IsNullOrEmpty(objUserSession?.Token))
            {
                responseMessage.Message = MessageConstant.Token;
                return false;

            }
            return true;
        }

    }
}
