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
    public class PermissionService : IPermissionService
    {
        private readonly POSDbContext _posDbContext;

        public PermissionService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Permission
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllPermission(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Permission> lstPermission = new List<Permission>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstPermission = await _posDbContext.Permission.OrderBy(x => x.PermissionID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstPermission.Count;


                responseMessage.ResponseObj = lstPermission;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllPermission");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllPermission");
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
        public async Task<ResponseMessage> GetPermissionById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Permission objPermission = new Permission();
                int permissionID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objPermission = await _posDbContext.Permission.FirstOrDefaultAsync(x => x.PermissionID == permissionID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objPermission;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetPermissionById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetPermissionById");
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
        public async Task<ResponseMessage> DeletePermission(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Permission objPermission = new Permission();
                int permissionID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objPermission = await _posDbContext.Permission.AsNoTracking().FirstOrDefaultAsync(x => x.PermissionID == permissionID);

                if (objPermission.PermissionID > 0)
                {
                    if (objPermission.PermissionID == 1)
                    {
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                        responseMessage.Message = "Permission 'Global Admin' can not be deleted";
                        return responseMessage;
                    }
                    _posDbContext.Permission.Remove(objPermission);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Permission not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeletePermission");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeletePermission");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Permission
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SavePermission(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Permission objPermission = JsonConvert.DeserializeObject<Permission>(requestMessage?.RequestObj.ToString());
                if (objPermission != null)
                {
                    if (objPermission.PermissionID > 0)
                    {
                        Permission existingPermission = await _posDbContext.Permission.AsNoTracking().FirstOrDefaultAsync(x => x.PermissionID == objPermission.PermissionID);
                        if (existingPermission != null)
                        {
                            actionType = (int)Enums.ActionType.Update;
                            objPermission.CreatedDate = existingPermission.CreatedDate;
                            objPermission.CreatedBy = existingPermission.CreatedBy;
                            objPermission.UpdatedDate = DateTime.Now;
                            objPermission.UpdatedBy = requestMessage.UserID;
                            _posDbContext.Permission.Update(objPermission);

                        }
                    }
                    else
                    {
                        //objPermission.Status = (int)Enums.Status.Active;
                        objPermission.CreatedDate = DateTime.Now;
                        objPermission.CreatedBy = requestMessage.UserID;
                        await _posDbContext.Permission.AddAsync(objPermission);

                    }


                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = objPermission;
                    responseMessage.Message = MessageConstant.SavedSuccessfully;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                    //Log write
                    LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SavePermission");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SavePermission");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

    }
}
