using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using POS.Common.Constants;
using POS.Common.DTO;
using POS.Common.Enums;
using POS.Common.Helper;
using POS.Common.Models;
using POS.DataAccess;
using POS.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly POSDbContext _posDbContext;

        public RoleService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Role
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllRole(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Role> lstRole = new List<Role>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstRole = await _posDbContext.Role.OrderBy(x => x.RoleID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstRole.Count;


                responseMessage.ResponseObj = lstRole;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllRole");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllRole");
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
        public async Task<ResponseMessage> GetRoleById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Role objRole = new Role();
                int roleID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objRole = await _posDbContext.Role.FirstOrDefaultAsync(x => x.RoleID == roleID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objRole;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetRoleById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetRoleById");
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
        public async Task<ResponseMessage> DeleteRole(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Role objRole = new Role();
                int roleID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objRole = await _posDbContext.Role.AsNoTracking().FirstOrDefaultAsync(x => x.RoleID == roleID);

                if (objRole.RoleID > 0)
                {
                    if (objRole.RoleID == 1)
                    {
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                        responseMessage.Message = "Role 'Global Admin' can not be deleted";
                        return responseMessage;
                    }
                    _posDbContext.Role.Remove(objRole);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Role not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteRole");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteRole");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Role
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveRole(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Role objRole = JsonConvert.DeserializeObject<Role>(requestMessage?.RequestObj.ToString());
                if (objRole != null)
                {
                    if (objRole.RoleID > 0)
                    {
                        Role existingRole = await _posDbContext.Role.AsNoTracking().FirstOrDefaultAsync(x => x.RoleID == objRole.RoleID);
                        if (existingRole != null)
                        {
                            actionType = (int)Enums.ActionType.Update;
                            objRole.CreatedDate = existingRole.CreatedDate;
                            objRole.CreatedBy = existingRole.CreatedBy;
                            objRole.UpdatedDate = DateTime.Now;
                            objRole.UpdatedBy = requestMessage.UserID;
                            _posDbContext.Role.Update(objRole);

                        }
                    }
                    else
                    {
                        //objRole.Status = (int)Enums.Status.Active;
                        objRole.CreatedDate = DateTime.Now;
                        objRole.CreatedBy = requestMessage.UserID;
                        await _posDbContext.Role.AddAsync(objRole);

                    }


                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = objRole;
                    responseMessage.Message = MessageConstant.SavedSuccessfully;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                    //Log write
                    LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveRole");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveRole");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

    }
}
