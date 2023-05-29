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
using POS.Common.VM;

namespace POS.Services
{
    public class PurchaseService: IPurchaseService
    {
        private readonly POSDbContext _posDbContext;

        public PurchaseService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Purchase
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllPurchase(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<VMPurchase> lstPurchase = new List<VMPurchase>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstPurchase = await _posDbContext.VMPurchase.OrderBy(x => x.CreatedDate).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstPurchase.Count;


                responseMessage.ResponseObj = lstPurchase;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllPurchase");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllPurchase");
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
        public async Task<ResponseMessage> GetPurchaseById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Purchase objPurchase = new Purchase();
                int purchaseID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objPurchase = await _posDbContext.Purchase.FirstOrDefaultAsync(x => x.PurchaseID == purchaseID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objPurchase;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetPurchaseById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetPurchaseById");
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
        public async Task<ResponseMessage> GetPurchaseByPurchaseCode(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Purchase objPurchase = new Purchase();
                string purchaseCode = requestMessage?.RequestObj.ToString();

                objPurchase = await _posDbContext.Purchase.AsNoTracking().FirstOrDefaultAsync(x => x.PurchaseCode == purchaseCode);
                responseMessage.ResponseObj = objPurchase;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetPurchaseByPurchaseCode");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetPurchaseById");
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
        public async Task<ResponseMessage> DeletePurchase(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                VMPurchase objPurchase = JsonConvert.DeserializeObject<VMPurchase>(requestMessage?.RequestObj.ToString());

                Purchase existingPurchase = await _posDbContext.Purchase.AsNoTracking().FirstOrDefaultAsync(x => x.PurchaseCode == objPurchase.PurchaseCode);
               
                if (existingPurchase.PurchaseID > 0)
                {
                    List<PurchaseProductMapping> lstPurchaseProductMapping = await _posDbContext.PurchaseProductMapping.AsNoTracking().Where(x => x.PurchaseID == existingPurchase.PurchaseID).ToListAsync();

                    if (lstPurchaseProductMapping.Count > 0)
                    {
                        foreach (var item in lstPurchaseProductMapping)
                        {
                            Product objProduct = await _posDbContext.Product.AsNoTracking().Where(x => x.ProductID == item.ProductID).FirstOrDefaultAsync();
                            if (objProduct != null)
                            {
                                objProduct.Qty = (int)(objProduct.Qty - item.Qty);
                                objProduct.UpdatedBy = requestMessage.UserID;
                                objProduct.UpdatedDate = DateTime.Now;
                                _posDbContext.Product.Update(objProduct);
                            }
                            _posDbContext.PurchaseProductMapping.Remove(item);
                        }
                    }
                    if (existingPurchase.PaymentAmount > 0 && existingPurchase.PaymentType > 0)
                    {
                        Expense objExpense = await _posDbContext.Expense.AsNoTracking().FirstOrDefaultAsync(x => x.PurchaseID == existingPurchase.PurchaseID);
                        if (objExpense != null)
                        {
                            List<AccountStatement> exist = await _posDbContext.AccountStatement.AsNoTracking().Where(x => x.ExpenseID == objExpense.ExpenseID).ToListAsync();
                            if (exist.Count > 0)
                            {
                                _posDbContext.AccountStatement.RemoveRange(exist);
                            }
                            _posDbContext.Expense.Remove(objExpense);
                        }
                    }
                    _posDbContext.Purchase.Remove(existingPurchase);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Purchase not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeletePurchase");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeletePurchase");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Purchase
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SavePurchase(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {
                Purchase objPurchase = JsonConvert.DeserializeObject<Purchase>(requestMessage?.RequestObj.ToString());
                if (objPurchase != null)
                {
                    if (CheckedValidation(objPurchase, responseMessage))
                    {
                        if (objPurchase.PurchaseID > 0)
                        {
                            Purchase existingPurchase = await this._posDbContext.Purchase.AsNoTracking().FirstOrDefaultAsync(x => x.PurchaseID == objPurchase.PurchaseID);
                            if (existingPurchase != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objPurchase.CreatedDate = existingPurchase.CreatedDate;
                                objPurchase.CreatedBy = existingPurchase.CreatedBy;
                                objPurchase.UpdatedDate = DateTime.Now;
                                objPurchase.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Purchase.Update(objPurchase);

                                List<PurchaseProductMapping> existProduct = await _posDbContext.PurchaseProductMapping.AsNoTracking().Where(x => x.PurchaseID == objPurchase.PurchaseID).ToListAsync();
                                if (existProduct.Count > 0)
                                {
                                    _posDbContext.PurchaseProductMapping.RemoveRange((IEnumerable<PurchaseProductMapping>)existingPurchase);
                                }


                            }
                        }
                        else
                        {
                            // Get the current timestamp
                            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            // Generate a random number using the timestamp as a seed
                            //var randomNumber = new Random(timestamp.GetHashCode()).Next();
                            objPurchase.PurchaseCode = timestamp.ToString();
                            
                            objPurchase.CreatedDate = DateTime.Now;
                            objPurchase.CreatedBy = requestMessage.UserID;
                            await _posDbContext.Purchase.AddAsync(objPurchase);

                        }

                        await _posDbContext.SaveChangesAsync();

                        //list of product added in the DB
                        if (objPurchase.lstProduct.Count > 0)
                        {
                            foreach (VMProduct product in objPurchase.lstProduct)
                            {
                                Product existProduct = await _posDbContext.Product.AsNoTracking().Where(x => x.SKU == product.SKU).FirstOrDefaultAsync();

                                if (existProduct != null)
                                {
                                    PurchaseProductMapping objPurchaseProductMapping = new PurchaseProductMapping();
                                    objPurchaseProductMapping.ProductID = existProduct.ProductID;
                                    objPurchaseProductMapping.UnitPrice = product.PurchasePrice;
                                    objPurchaseProductMapping.Qty = product.Qty;
                                    objPurchaseProductMapping.TotalPrice = (product.Qty * product.PurchasePrice);
                                    objPurchaseProductMapping.PurchaseID = objPurchase.PurchaseID;

                                    await _posDbContext.PurchaseProductMapping.AddAsync(objPurchaseProductMapping);

                                    // update quantity of purchased product
                                    existProduct.Qty = (int)(existProduct.Qty + (product.Qty ?? 0));
                                    existProduct.UpdatedBy = requestMessage.UserID;
                                    existProduct.UpdatedDate = DateTime.Now;
                                    _posDbContext.Product.Update(existProduct);

                                }
                            }
                            await _posDbContext.SaveChangesAsync();
                        }

                        //payment added in the DB
                        if (objPurchase.PaymentAmount > 0 && objPurchase.PaymentType > 0)
                        {
                            Expense objExpense = new Expense();
                            Expense existExpense = await _posDbContext.Expense.AsNoTracking().Where(x => x.PurchaseID == objPurchase.PurchaseID).FirstOrDefaultAsync();

                            if (existExpense != null)
                            {
                                objExpense.AccountID = (int)objPurchase.PaymentType;
                                objExpense.Amount = (double)objPurchase.PaymentAmount;
                                objExpense.Description = objPurchase.PaymentNote;
                                objExpense.PurchaseID = existExpense.PurchaseID;
                                objExpense.ExpenseTitle = existExpense.ExpenseTitle;

                                objExpense.UpdatedDate = DateTime.Now;
                                objExpense.UpdatedBy = requestMessage.UserID;
                                objExpense.CreatedDate = existExpense.CreatedDate;
                                objExpense.CreatedBy = existExpense.CreatedBy;

                                _posDbContext.Expense.Update(objExpense);

                                List<AccountStatement> exist = await _posDbContext.AccountStatement.AsNoTracking().Where(x => x.ExpenseID == objExpense.ExpenseID).ToListAsync();
                                if (exist.Count > 0)
                                {
                                    _posDbContext.AccountStatement.RemoveRange(exist);
                                }
                                await _posDbContext.SaveChangesAsync();
                            }
                            else
                            {
                                objExpense.AccountID = (int)objPurchase.PaymentType;
                                objExpense.Amount = (double)objPurchase.PaymentAmount;
                                objExpense.Description = objPurchase.PaymentNote;
                                objExpense.PurchaseID = objPurchase.PurchaseID;
                                objExpense.ExpenseTitle = "Purchase - " + objPurchase.PurchaseCode;

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
                        }


                        responseMessage.ResponseObj = objPurchase;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SavePurchase");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SavePurchase");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objPurchase"></param>
        /// <returns></returns>
        private bool CheckedValidation(Purchase objPurchase, ResponseMessage responseMessage)
        {

            bool result = true;
            Purchase existingPurchase = new Purchase();

            existingPurchase = _posDbContext.Purchase.Where(x => x.PurchaseCode == objPurchase.PurchaseCode).AsNoTracking().FirstOrDefault();
            if (existingPurchase != null && objPurchase.PurchaseID == 0)
            {
                responseMessage.Message = "Purchase code is already exist";
                return false;
            }
            
            return true;
        }
    }
}
