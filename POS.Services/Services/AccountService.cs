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
using static POS.Common.Enums.Enums;
using POS.Common.VM;

namespace POS.Services
{
    public class AccountService : IAccountService
    {
        private readonly POSDbContext _posDbContext;

        public AccountService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Account
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllAccount(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Account> lstAccount = new List<Account>();
                List<VMGetAccountBalanceExpense> lstAccountBalanceExpesne = new List<VMGetAccountBalanceExpense>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstAccount = await _posDbContext.Account.OrderBy(x => x.AccountID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstAccount.Count;

                lstAccountBalanceExpesne = await _posDbContext.VMGetAccountBalanceExpense.Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();

                responseMessage.ResponseObj = lstAccountBalanceExpesne;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllAccount");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllAccount");
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
        public async Task<ResponseMessage> GetAccountById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Account objAccount = new Account();
                int accountID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objAccount = await _posDbContext.Account.FirstOrDefaultAsync(x => x.AccountID == accountID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objAccount;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAccountById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAccountById");
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
        public async Task<ResponseMessage> DeleteAccount(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Account objAccount = new Account();
                int accountID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objAccount = await _posDbContext.Account.AsNoTracking().FirstOrDefaultAsync(x => x.AccountID == accountID);

                if (objAccount.AccountID > 0)
                {

                    _posDbContext.Account.Remove(objAccount);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Account not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteAccount");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteAccount");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Account
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveAccount(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {
                Account objAccount = JsonConvert.DeserializeObject<Account>(requestMessage?.RequestObj.ToString());

                if (objAccount != null)
                {
                    if (CheckedValidation(objAccount, responseMessage))
                    {
                        if (objAccount.AccountID > 0)
                        {
                            Account existingAccount = await this._posDbContext.Account.AsNoTracking().FirstOrDefaultAsync(x => x.AccountID == objAccount.AccountID);
                            if (existingAccount != null)
                            {
                                actionType = (int)Enums.ActionType.Update;

                                _posDbContext.Account.Update(objAccount);

                            }
                        }
                        else
                        {
                            //objAccount.Status = (int)Enums.Status.Active;
                            await _posDbContext.Account.AddAsync(objAccount);

                        }

                        await _posDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = objAccount;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveAccount");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveAccount");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        public async Task<ResponseMessage> AddBalance(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Update;
            try
            {
                Account objAccount = JsonConvert.DeserializeObject<Account>(requestMessage?.RequestObj.ToString());

                if (objAccount.AccountID > 0 & objAccount.Balance > 0)
                {
                    Account existAccount = await _posDbContext.Account.AsNoTracking().FirstOrDefaultAsync(x => x.AccountID == objAccount.AccountID);
                    if (existAccount != null)
                    {
                        AccountStatement objAccountStatement = new AccountStatement();
                        objAccountStatement.AccountID = objAccount.AccountID;
                        objAccountStatement.InBalance = objAccount.Balance;
                        objAccountStatement.CreatedDate = DateTime.Now;
                        objAccountStatement.CreatedBy = requestMessage.UserID;

                        await _posDbContext.AccountStatement.AddAsync(objAccountStatement);

                        existAccount.Balance = existAccount.Balance + objAccount.Balance;
                        _posDbContext.Account.Update(existAccount);

                        await _posDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = objAccountStatement;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                        responseMessage.Message = "Balance added successfully";

                    }
                    else
                    {
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                        responseMessage.Message = "Account not found";
                    }
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Something went wrong";
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "AddBalance");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }
            return responseMessage;
        }

        public async Task<ResponseMessage> ShowAccountStatement(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            Account objAccount = JsonConvert.DeserializeObject<Account>(requestMessage?.RequestObj.ToString());
            int actionType = (int)Enums.ActionType.View;
            try
            {
                List<VMAccountStatement> lstVMAccountStatement = new List<VMAccountStatement>();
                if (objAccount.AccountID > 0)
                {
                    string query = @"
                        SELECT ast.CreatedDate AS TransactionDate, ast.InBalance, ast.OutBalance,
                        (SELECT SUM(COALESCE(InBalance, 0) - COALESCE(OutBalance, 0))
                        FROM AccountStatement WHERE AccountID = ast.AccountID AND CreatedDate <= ast.CreatedDate) AS Balance
                        FROM AccountStatement ast WHERE ast.AccountID = " + objAccount.AccountID + " ORDER BY ast.CreatedDate";
                    lstVMAccountStatement = await _posDbContext.VMAccountStatement.FromSqlRaw<VMAccountStatement>(query).ToListAsync();
                    var totalInBalance = lstVMAccountStatement.Sum(x => x.InBalance);
                    var totalOutBalance = lstVMAccountStatement.Sum(x => x.OutBalance);

                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.ResponseObj = new { lstVMAccountStatement, totalInBalance, totalOutBalance };
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Account not found";
                }

            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "ShowAccountStatement");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }
            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objAccount"></param>
        /// <returns></returns>
        private bool CheckedValidation(Account objAccount, ResponseMessage responseMessage)
        {

            bool result = true;
            Account existingAccount = new Account();


            if (String.IsNullOrEmpty(objAccount.AccountTitle))
            {
                responseMessage.Message = "Account title is required";
                return false;
            }

            return true;
        }
    }
}
