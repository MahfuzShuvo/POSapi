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
    public class UnitService: IUnitService
    {
        private readonly POSDbContext _posDbContext;

        public UnitService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Unit
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllUnit(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Unit> lstUnit = new List<Unit>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstUnit = await _posDbContext.Unit.OrderBy(x => x.UnitID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstUnit.Count;


                responseMessage.ResponseObj = lstUnit;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllUnit");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllUnit");
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
        public async Task<ResponseMessage> GetUnitById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Unit objUnit = new Unit();
                int unitID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objUnit = await _posDbContext.Unit.FirstOrDefaultAsync(x => x.UnitID == unitID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objUnit;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetUnitById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetUnitById");
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
        public async Task<ResponseMessage> DeleteUnit(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Unit objUnit = new Unit();
                int unitID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objUnit = await _posDbContext.Unit.AsNoTracking().FirstOrDefaultAsync(x => x.UnitID == unitID);
               
                if (objUnit.UnitID > 0)
                {

                    _posDbContext.Unit.Remove(objUnit);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Unit not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteUnit");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteUnit");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Unit
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveUnit(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Unit objUnit = JsonConvert.DeserializeObject<Unit>(requestMessage?.RequestObj.ToString());
                if (objUnit != null)
                {
                    if (CheckedValidation(objUnit, responseMessage))
                    {
                        if (objUnit.UnitID > 0)
                        {
                            Unit existingUnit = await this._posDbContext.Unit.AsNoTracking().FirstOrDefaultAsync(x => x.UnitID == objUnit.UnitID);
                            if (existingUnit != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objUnit.CreatedDate = existingUnit.CreatedDate;
                                objUnit.CreatedBy = existingUnit.CreatedBy;
                                objUnit.UpdatedDate = DateTime.Now;
                                objUnit.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Unit.Update(objUnit);

                            }
                        }
                        else
                        {
                            //objUnit.Status = (int)Enums.Status.Active;
                            objUnit.CreatedDate = DateTime.Now;
                            objUnit.CreatedBy = requestMessage.UserID;
                            await _posDbContext.Unit.AddAsync(objUnit);

                        }

                     
                        await _posDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = objUnit;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveUnit");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveUnit");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objUnit"></param>
        /// <returns></returns>
        private bool CheckedValidation(Unit objUnit, ResponseMessage responseMessage)
        {

            bool result = true;
            Unit existingUnit = new Unit();


            if (String.IsNullOrEmpty(objUnit.UnitName))
            {
                responseMessage.Message = "Unit name required";
                return false;
            }
            
            return true;
        }
    }
}
