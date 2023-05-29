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
    public class SystemUserService: ISystemUserService
    {
        private readonly POSDbContext _posDbContext;

        public SystemUserService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all System User
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllSystemUser(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<SystemUser> lstSystemUser = new List<SystemUser>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstSystemUser = await _posDbContext.SystemUser.OrderBy(x => x.SystemUserID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstSystemUser.Count;

                foreach (SystemUser user in lstSystemUser)
                {
                    user.Password = string.Empty;
                }


                responseMessage.ResponseObj = lstSystemUser;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllSystemUser");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllSystemUser");
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
        public async Task<ResponseMessage> GetSystemUserById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                SystemUser objSystemUser = new SystemUser();
                int systemUserID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objSystemUser = await _posDbContext.SystemUser.FirstOrDefaultAsync(x => x.SystemUserID == systemUserID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objSystemUser;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetSystemUserById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetSystemUserById");
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
        public async Task<ResponseMessage> DeleteSystemUser(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                SystemUser objSystemUser = new SystemUser();
                int systemUserID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objSystemUser = await _posDbContext.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.SystemUserID == systemUserID);
               
                if (objSystemUser.SystemUserID > 0)
                {

                    _posDbContext.SystemUser.Remove(objSystemUser);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "User not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteSystemUser");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteSystemUser");
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
        public async Task<ResponseMessage> SaveSystemUser(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                SystemUser objSystemUser = JsonConvert.DeserializeObject<SystemUser>(requestMessage?.RequestObj.ToString());
                if (objSystemUser != null)
                {
                    if (CheckedValidation(objSystemUser, responseMessage))
                    {
                        if (objSystemUser.SystemUserID > 0)
                        {
                            SystemUser existingSystemUser = await this._posDbContext.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.SystemUserID == objSystemUser.SystemUserID);
                            if (existingSystemUser != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objSystemUser.Password = (string.IsNullOrEmpty(objSystemUser.Password)) ? existingSystemUser.Password : BCrypt.Net.BCrypt.HashPassword(objSystemUser.Password);
                                objSystemUser.CreatedDate = existingSystemUser.CreatedDate;
                                objSystemUser.CreatedBy = existingSystemUser.CreatedBy;
                                objSystemUser.UpdatedDate = DateTime.Now;
                                objSystemUser.UpdatedBy = requestMessage.UserID;
                                _posDbContext.SystemUser.Update(objSystemUser);

                            }
                        }
                        else
                        {
                            //objSystemUser.Status = (int)Enums.Status.Active;
                            objSystemUser.CreatedDate = DateTime.Now;
                            objSystemUser.CreatedBy = requestMessage.UserID;
                            objSystemUser.Password = BCrypt.Net.BCrypt.HashPassword(objSystemUser.Password);
                            await _posDbContext.SystemUser.AddAsync(objSystemUser);

                        }

                     
                        await _posDbContext.SaveChangesAsync();

                        objSystemUser.Password = string.Empty;
                        responseMessage.ResponseObj = objSystemUser;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveSystemUser");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveSystemUser");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objSystemUser"></param>
        /// <returns></returns>
        private bool CheckedValidation(SystemUser objSystemUser, ResponseMessage responseMessage)
        {

            bool result = true;
            SystemUser existingSystemUser = new SystemUser();

            existingSystemUser = _posDbContext.SystemUser.Where(x => x.Username == objSystemUser.Username && x.SystemUserID != objSystemUser.SystemUserID).AsNoTracking().FirstOrDefault();

            if (String.IsNullOrEmpty(objSystemUser.Username))
            {
                responseMessage.Message = "Username required";
                return false;
            }
            if (String.IsNullOrEmpty(objSystemUser.Email))
            {
                responseMessage.Message = "Email required";
                return false;
            }
            if (String.IsNullOrEmpty(objSystemUser.Password) && objSystemUser.SystemUserID == 0)
            {
                responseMessage.Message = "Password required";
                return false;
            }
            existingSystemUser = _posDbContext.SystemUser.Where(x => x.Username.ToLower() == objSystemUser.Username.ToLower()).AsNoTracking().FirstOrDefault();
            if (existingSystemUser != null && objSystemUser.SystemUserID == 0)
            {
                responseMessage.Message = "Username already exist";
                return false;
            }
            existingSystemUser = _posDbContext.SystemUser.Where(x => x.Email == objSystemUser.Email).AsNoTracking().FirstOrDefault();
            if (existingSystemUser != null && objSystemUser.SystemUserID == 0)
            {
                responseMessage.Message = "Email already exist";
                return false;
            }
            existingSystemUser = _posDbContext.SystemUser.Where(x => x.PhoneNumber == objSystemUser.PhoneNumber).AsNoTracking().FirstOrDefault();
            if (existingSystemUser != null && objSystemUser.SystemUserID == 0)
            {
                responseMessage.Message = "Phone number already exist";
                return false;
            }
            return true;
        }

    }


}
