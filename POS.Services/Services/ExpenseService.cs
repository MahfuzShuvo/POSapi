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
    public class ExpenseService : IExpenseService
    {
        private readonly POSDbContext _posDbContext;

        public ExpenseService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Expense
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllExpense(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Expense> lstExpense = new List<Expense>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstExpense = await _posDbContext.Expense.OrderBy(x => x.ExpenseID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstExpense.Count;


                responseMessage.ResponseObj = lstExpense;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllExpense");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllExpense");
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
        public async Task<ResponseMessage> GetExpenseById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Expense objExpense = new Expense();
                int expenseID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objExpense = await _posDbContext.Expense.FirstOrDefaultAsync(x => x.ExpenseID == expenseID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objExpense;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetExpenseById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetExpenseById");
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
        public async Task<ResponseMessage> DeleteExpense(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Expense objExpense = new Expense();
                int expenseID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objExpense = await _posDbContext.Expense.AsNoTracking().FirstOrDefaultAsync(x => x.ExpenseID == expenseID);

                if (objExpense.ExpenseID > 0)
                {
                    List<AccountStatement> exist = await _posDbContext.AccountStatement.AsNoTracking().Where(x => x.ExpenseID == objExpense.ExpenseID).ToListAsync();
                    if (exist.Count > 0)
                    {
                        _posDbContext.AccountStatement.RemoveRange(exist);
                    }
                    _posDbContext.Expense.Remove(objExpense);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Expense not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteExpense");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteExpense");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Expense
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveExpense(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Expense objExpense = JsonConvert.DeserializeObject<Expense>(requestMessage?.RequestObj.ToString());
                if (objExpense != null)
                {
                    if (CheckedValidation(objExpense, responseMessage))
                    {
                        if (objExpense.ExpenseID > 0)
                        {
                            Expense existingExpense = await this._posDbContext.Expense.AsNoTracking().FirstOrDefaultAsync(x => x.ExpenseID == objExpense.ExpenseID);
                            if (existingExpense != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objExpense.CreatedDate = existingExpense.CreatedDate;
                                objExpense.CreatedBy = existingExpense.CreatedBy;
                                objExpense.UpdatedDate = DateTime.Now;
                                objExpense.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Expense.Update(objExpense);

                                AccountStatement exist = await _posDbContext.AccountStatement.AsNoTracking().FirstOrDefaultAsync(x => x.ExpenseID == objExpense.ExpenseID);
                                _posDbContext.AccountStatement.Remove(exist);
                                await _posDbContext.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            //objExpense.Status = (int)Enums.Status.Active;
                            objExpense.CreatedDate = DateTime.Now;
                            objExpense.CreatedBy = requestMessage.UserID;
                            await _posDbContext.Expense.AddAsync(objExpense);
                            await _posDbContext.SaveChangesAsync();
                        }
                        AccountStatement objAccountStatement = new AccountStatement();
                        objAccountStatement.ExpenseID = objExpense.ExpenseID;
                        objAccountStatement.AccountID = objExpense.AccountID;
                        objAccountStatement.OutBalance = objExpense.Amount;
                        objAccountStatement.CreatedDate = DateTime.Now;
                        objAccountStatement.CreatedBy = requestMessage.UserID;

                        await _posDbContext.AccountStatement.AddAsync(objAccountStatement);
                        await _posDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = objExpense;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveExpense");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveExpense");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objExpense"></param>
        /// <returns></returns>
        private bool CheckedValidation(Expense objExpense, ResponseMessage responseMessage)
        {

            bool result = true;
            Expense existingExpense = new Expense();


            if (String.IsNullOrEmpty(objExpense.ExpenseTitle))
            {
                responseMessage.Message = "Expense title is required";
                return false;
            }

            return true;
        }
    }
}
