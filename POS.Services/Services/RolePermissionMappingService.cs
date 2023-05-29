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
    public class RolePermissionMappingService : IRolePermissionMappingService
    {
        private readonly POSDbContext _posDbContext;

        public RolePermissionMappingService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all RolePermissionMapping
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllRolePermissionMapping(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<RolePermissionMapping> lstRolePermissionMapping = new List<RolePermissionMapping>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstRolePermissionMapping = await _posDbContext.RolePermissionMapping.OrderBy(x => x.RolePermissionMappingID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstRolePermissionMapping.Count;


                responseMessage.ResponseObj = lstRolePermissionMapping;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllRolePermissionMapping");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllRolePermissionMapping");
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
        public async Task<ResponseMessage> GetRolePermissionMappingById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                RolePermissionMapping objRolePermissionMapping = new RolePermissionMapping();
                int rolePermissionMappingID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objRolePermissionMapping = await _posDbContext.RolePermissionMapping.FirstOrDefaultAsync(x => x.RolePermissionMappingID == rolePermissionMappingID);
                responseMessage.ResponseObj = objRolePermissionMapping;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetRolePermissionMappingById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetRolePermissionMappingById");
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
        public async Task<ResponseMessage> DeleteRolePermissionMapping(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                RolePermissionMapping objRolePermissionMapping = new RolePermissionMapping();
                int rolePermissionMappingID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objRolePermissionMapping = await _posDbContext.RolePermissionMapping.AsNoTracking().FirstOrDefaultAsync(x => x.RolePermissionMappingID == rolePermissionMappingID);

                if (objRolePermissionMapping.RolePermissionMappingID > 0)
                {
                    if (objRolePermissionMapping.RolePermissionMappingID == 1)
                    {
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                        responseMessage.Message = "RolePermissionMapping 'Global Admin' can not be deleted";
                        return responseMessage;
                    }
                    _posDbContext.RolePermissionMapping.Remove(objRolePermissionMapping);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "RolePermissionMapping not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteRolePermissionMapping");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteRolePermissionMapping");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update RolePermissionMapping
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveRolePermissionMapping(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                RolePermissionMapping objRolePermissionMapping = JsonConvert.DeserializeObject<RolePermissionMapping>(requestMessage?.RequestObj.ToString());
                if (objRolePermissionMapping != null)
                {
                    if (!objRolePermissionMapping.IsChecked)
                    {
                        RolePermissionMapping existingRolePermissionMapping = await _posDbContext.RolePermissionMapping.AsNoTracking().FirstOrDefaultAsync(x => x.RoleID == objRolePermissionMapping.RoleID && x.PermissionID == objRolePermissionMapping.PermissionID);
                        _posDbContext.RolePermissionMapping.Remove(existingRolePermissionMapping);

                    } 
                    else
                    {
                        await _posDbContext.RolePermissionMapping.AddAsync(objRolePermissionMapping);
                    }


                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = objRolePermissionMapping;
                    responseMessage.Message = MessageConstant.SavedSuccessfully;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                    //Log write
                    LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveRolePermissionMapping");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveRolePermissionMapping");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

    }
}
