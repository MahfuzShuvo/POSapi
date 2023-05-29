using POS.Common.Constants;
using POS.Common.DTO;
using POS.Common.Enums;
using POS.Common.Helper;
using POS.Common.Models;
using POS.Common.VM;
using POS.DataAccess;
using POS.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static POS.Common.Enums.Enums;

namespace POS.Services.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly POSDbContext _posDbContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SecurityService(POSDbContext ctx, IServiceScopeFactory serviceScopeFactor)
        {
            _posDbContext = ctx;
            _serviceScopeFactory = serviceScopeFactor;

        }

        /// <summary>
        /// method for checking action.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> CheckPermission(string url)
        {
            //uncomment when implement action and permisson

            //Actions objAction = await this._posDbContext.Actions.Where(x => x.ActionName == url && x.Status == (int)Enums.Status.Active).FirstOrDefaultAsync();
            //if (objAction != null)
            //{
            //    return true;
            //}

            return await Task.FromResult(true);
        }

        public async Task<ResponseMessage> Login(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                VMLogin objVMLogin = JsonConvert.DeserializeObject<VMLogin>(requestMessage.RequestObj.ToString());
                SystemUser existingSystemUser = new SystemUser();
                if (objVMLogin == null || string.IsNullOrEmpty(objVMLogin.Username) || string.IsNullOrEmpty(objVMLogin.Password))
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Invalid username or password";
                    return responseMessage;
                }


                existingSystemUser = await _posDbContext.SystemUser.Where(x => x.Username == objVMLogin.Username && x.Status == (int)Enums.Status.Active).AsNoTracking().FirstOrDefaultAsync();

                if (existingSystemUser == null || !BCrypt.Net.BCrypt.Verify(objVMLogin.Password, existingSystemUser.Password))
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Invalid username or password";
                    return responseMessage;
                }
                else
                {
                    objVMLogin.FullName = existingSystemUser.FullName;
                    objVMLogin.SystemUserID = existingSystemUser.SystemUserID;
                    objVMLogin.RoleID = existingSystemUser.RoleID;
                }

                
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                responseMessage.Message = MessageConstant.LoginSuccess;

                objVMLogin.Password = String.Empty;
                responseMessage.ResponseObj = objVMLogin;

                //Log write
                LogHelper.WriteLog(requestMessage.RequestObj, (int)Enums.ActionType.Login, objVMLogin.SystemUserID, "Login");

            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Login, 0, JsonConvert.SerializeObject(requestMessage.RequestObj), "Login");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;

        }

        /// <summary>
        /// Method for log out.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> Logout(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {

                List<UserSession> lstUserSession = new List<UserSession>();

                lstUserSession = await _posDbContext.UserSession.AsNoTracking().Where(x => x.SystemUserID == requestMessage.UserID && x.Status == (int)Enums.Status.Active).ToListAsync();

                if (lstUserSession.Count > 0)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetService<POSDbContext>();
                        foreach (var item in lstUserSession)
                        {
                            item.SessionEnd = DateTime.Now;
                            item.Status = (int)Enums.Status.Inactive;
                        }
                        db.UserSession.UpdateRange(lstUserSession);
                        await db.SaveChangesAsync();
                    }
                }


                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                responseMessage.Message = MessageConstant.LogOutSuccessfully;
                return responseMessage;
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Logout, 0, JsonConvert.SerializeObject(requestMessage.RequestObj), "Logout");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;

        }


        /// <summary>
        /// method for save user.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> Register(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Insert;
            try
            {
                VMRegister objVMRegister = JsonConvert.DeserializeObject<VMRegister>(requestMessage.RequestObj.ToString());

                if (objVMRegister == null)
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = MessageConstant.Invaliddatafound;
                    return responseMessage;
                }
                else
                {
                    if (CheckedValidation(objVMRegister, responseMessage))
                    {

                        if (objVMRegister.Password != objVMRegister.ConfirmPassword)
                        {
                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                            responseMessage.Message = "Confirm password not match";
                            return responseMessage;
                        }

                        SystemUser objSystemUser = new SystemUser();
                        objSystemUser.Status = (int)Enums.Status.Active;
                        objSystemUser.CreatedDate = DateTime.Now;
                        objSystemUser.CreatedBy = requestMessage.UserID;
                        objSystemUser.FullName = objVMRegister.FullName;
                        objSystemUser.Username = objVMRegister.Username;
                        objSystemUser.PhoneNumber = objVMRegister.PhoneNumber;
                        objSystemUser.Email = objVMRegister.Email;
                        objSystemUser.Address= objVMRegister.Address;
                        objSystemUser.City = objVMRegister.City;
                        objSystemUser.State= objVMRegister.State;
                        objSystemUser.Zip= objVMRegister.Zip;
                        objSystemUser.Password = BCrypt.Net.BCrypt.HashPassword(objVMRegister.Password);

                        await _posDbContext.SystemUser.AddAsync(objSystemUser);
                        await _posDbContext.SaveChangesAsync();

                        objSystemUser.Password = string.Empty;
                        responseMessage.ResponseObj = objSystemUser;
                        responseMessage.Message = MessageConstant.RegisterSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "Register");

                    }
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Register, 0, JsonConvert.SerializeObject(requestMessage.RequestObj), "Register");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objVMRegister"></param>
        /// <returns></returns>
        private bool CheckedValidation(VMRegister objVMRegister, ResponseMessage responseMessage)
        {

            SystemUser existingSystemUser = new SystemUser();


            if (String.IsNullOrEmpty(objVMRegister.Username))
            {
                responseMessage.Message = "Username required";
                return false;
            }
            if (String.IsNullOrEmpty(objVMRegister.Email))
            {
                responseMessage.Message = "Email required";
                return false;
            }
            if (String.IsNullOrEmpty(objVMRegister.Password))
            {
                responseMessage.Message = "Password required";
                return false;
            }
            existingSystemUser = _posDbContext.SystemUser.Where(x => x.Username == objVMRegister.Username).AsNoTracking().FirstOrDefault();
            if (existingSystemUser != null)
            {
                responseMessage.Message = "Username already exist";
                return false;
            }
            existingSystemUser = _posDbContext.SystemUser.Where(x => x.Email == objVMRegister.Email).AsNoTracking().FirstOrDefault();
            if (existingSystemUser != null)
            {
                responseMessage.Message = "Email already exist";
                return false;
            }
            existingSystemUser = _posDbContext.SystemUser.Where(x => x.PhoneNumber == objVMRegister.PhoneNumber).AsNoTracking().FirstOrDefault();
            if (existingSystemUser != null)
            {
                responseMessage.Message = "Phone number already exist";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Generate Token
        /// </summary>
        /// <param name="systemUser"></param>
        /// <returns></returns>
        private string GenerateToken(SystemUser systemUser)
        {
            string token = string.Empty;

            return token;
        }
    }
}
