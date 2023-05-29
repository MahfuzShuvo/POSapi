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
    public class SupplierService: ISupplierService
    {
        private readonly POSDbContext _posDbContext;

        public SupplierService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Supplier
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllSupplier(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Supplier> lstSupplier = new List<Supplier>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstSupplier = await _posDbContext.Supplier.OrderBy(x => x.SupplierID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstSupplier.Count;


                responseMessage.ResponseObj = lstSupplier;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllSupplier");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllSupplier");
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
        public async Task<ResponseMessage> GetSupplierById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Supplier objSupplier = new Supplier();
                int supplierID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objSupplier = await _posDbContext.Supplier.FirstOrDefaultAsync(x => x.SupplierID == supplierID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objSupplier;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetSupplierById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetSupplierById");
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
        public async Task<ResponseMessage> DeleteSupplier(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Supplier objSupplier = new Supplier();
                int supplierID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objSupplier = await _posDbContext.Supplier.AsNoTracking().FirstOrDefaultAsync(x => x.SupplierID == supplierID);
               
                if (objSupplier.SupplierID > 0)
                {

                    _posDbContext.Supplier.Remove(objSupplier);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Supplier not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteSupplier");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteSupplier");
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
        public async Task<ResponseMessage> SaveSupplier(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Supplier objSupplier = JsonConvert.DeserializeObject<Supplier>(requestMessage?.RequestObj.ToString());
                if (objSupplier != null)
                {
                    if (CheckedValidation(objSupplier, responseMessage))
                    {
                        if (objSupplier.SupplierID > 0)
                        {
                            Supplier existingSupplier = await this._posDbContext.Supplier.AsNoTracking().FirstOrDefaultAsync(x => x.SupplierID == objSupplier.SupplierID);
                            if (existingSupplier != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objSupplier.CreatedDate = existingSupplier.CreatedDate;
                                objSupplier.CreatedBy = existingSupplier.CreatedBy;
                                objSupplier.UpdatedDate = DateTime.Now;
                                objSupplier.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Supplier.Update(objSupplier);

                            }
                        }
                        else
                        {
                            //objSupplier.Status = (int)Enums.Status.Active;
                            objSupplier.CreatedDate = DateTime.Now;
                            objSupplier.CreatedBy = requestMessage.UserID;
                            await _posDbContext.Supplier.AddAsync(objSupplier);

                        }

                     
                        await _posDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = objSupplier;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveSupplier");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveSupplier");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objSupplier"></param>
        /// <returns></returns>
        private bool CheckedValidation(Supplier objSupplier, ResponseMessage responseMessage)
        {

            bool result = true;
            Supplier existingSupplier = new Supplier();


            if (String.IsNullOrEmpty(objSupplier.Email))
            {
                responseMessage.Message = "Email required";
                return false;
            }
            
            existingSupplier = _posDbContext.Supplier.Where(x => x.Email == objSupplier.Email).AsNoTracking().FirstOrDefault();
            if (existingSupplier != null && objSupplier.SupplierID == 0)
            {
                responseMessage.Message = "Email already exist";
                return false;
            }
            existingSupplier = _posDbContext.Supplier.Where(x => x.PhoneNumber == objSupplier.PhoneNumber).AsNoTracking().FirstOrDefault();
            if (existingSupplier != null && objSupplier.SupplierID == 0)
            {
                responseMessage.Message = "Phone number already exist";
                return false;
            }
            return true;
        }
    }
}
