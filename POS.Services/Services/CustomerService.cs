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
    public class CustomerService: ICustomerService
    {
        private readonly POSDbContext _posDbContext;

        public CustomerService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Customer
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllCustomer(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Customer> lstCustomer = new List<Customer>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstCustomer = await _posDbContext.Customer.OrderBy(x => x.CustomerID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstCustomer.Count;


                responseMessage.ResponseObj = lstCustomer;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllCustomer");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllCustomer");
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
        public async Task<ResponseMessage> GetCustomerById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Customer objCustomer = new Customer();
                int customerID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objCustomer = await _posDbContext.Customer.FirstOrDefaultAsync(x => x.CustomerID == customerID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objCustomer;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetCustomerById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetCustomerById");
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
        public async Task<ResponseMessage> DeleteCustomer(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Customer objCustomer = new Customer();
                int customerID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objCustomer = await _posDbContext.Customer.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerID == customerID);
               
                if (objCustomer.CustomerID > 0)
                {

                    _posDbContext.Customer.Remove(objCustomer);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Customer not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteCustomer");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteCustomer");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Customer
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveCustomer(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Customer objCustomer = JsonConvert.DeserializeObject<Customer>(requestMessage?.RequestObj.ToString());
                if (objCustomer != null)
                {
                    if (CheckedValidation(objCustomer, responseMessage))
                    {
                        if (objCustomer.CustomerID > 0)
                        {
                            Customer existingCustomer = await this._posDbContext.Customer.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerID == objCustomer.CustomerID);
                            if (existingCustomer != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objCustomer.CreatedDate = existingCustomer.CreatedDate;
                                objCustomer.CreatedBy = existingCustomer.CreatedBy;
                                objCustomer.UpdatedDate = DateTime.Now;
                                objCustomer.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Customer.Update(objCustomer);

                            }
                        }
                        else
                        {
                            //objCustomer.Status = (int)Enums.Status.Active;
                            objCustomer.CreatedDate = DateTime.Now;
                            objCustomer.CreatedBy = requestMessage.UserID;
                            await _posDbContext.Customer.AddAsync(objCustomer);

                        }

                     
                        await _posDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = objCustomer;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveCustomer");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveCustomer");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objCustomer"></param>
        /// <returns></returns>
        private bool CheckedValidation(Customer objCustomer, ResponseMessage responseMessage)
        {

            bool result = true;
            Customer existingCustomer = new Customer();


            if (String.IsNullOrEmpty(objCustomer.Email))
            {
                responseMessage.Message = "Email required";
                return false;
            }
            existingCustomer = _posDbContext.Customer.Where(x => x.Email == objCustomer.Email).AsNoTracking().FirstOrDefault();
            if (existingCustomer != null && objCustomer.CustomerID == 0)
            {
                responseMessage.Message = "Email already exist";
                return false;
            }
            existingCustomer = _posDbContext.Customer.Where(x => x.PhoneNumber == objCustomer.PhoneNumber).AsNoTracking().FirstOrDefault();
            if (existingCustomer != null && objCustomer.CustomerID == 0)
            {
                responseMessage.Message = "Phone number already exist";
                return false;
            }
            return true;
        }
    }
}
