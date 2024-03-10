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
using Microsoft.Extensions.Configuration;

namespace POS.Services
{
    public class SalesService : ISalesService
    {
        private readonly POSDbContext _posDbContext;
        private readonly IConfiguration _configuration;

        public SalesService(POSDbContext ctx, IConfiguration configuration)
        {
            _posDbContext = ctx;
            _configuration = configuration;
        }

        /// <summary>
        /// Get all Sales
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllSales(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<VMSales> lstSales = new List<VMSales>();
                int branchID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstSales = await _posDbContext.VMSales.Where(x=> x.BranchID == branchID && x.Status == (int)Enums.Status.Active).OrderBy(x => x.CreatedDate).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstSales.Count;

                foreach (VMSales sales in lstSales)
                {
                    Sales objSales = _posDbContext.Sales.AsNoTracking().Where(x => x.SalesCode == sales.SalesCode).FirstOrDefault();
                    if (objSales != null)
                    {
                        List<SalesProductMapping> lstSalesProductMapping = await _posDbContext.SalesProductMapping.Where(x => x.SalesID == objSales.SalesID).ToListAsync();
                        if (lstSalesProductMapping.Count > 0)
                        {
                            List<Product> lstProduct = new List<Product>();

                            lstProduct = await _posDbContext.Product.Where(p =>
                                        lstSalesProductMapping.Select(spm => spm.ProductID).Contains(p.ProductID)).ToListAsync();
                            if (lstProduct.Count > 0)
                            {
                                foreach (Product objProduct in lstProduct)
                                {
                                    VMProduct product = new VMProduct();
                                    product = JsonConvert.DeserializeObject<VMProduct>(JsonConvert.SerializeObject(objProduct));


                                    if (product != null)
                                    {
                                        if (!string.IsNullOrEmpty(product.Image))
                                        {
                                            string getshowurl = _configuration.GetSection("attachments").GetSection("showfilepath").Value;
                                            product.Image = getshowurl + product.Image;
                                        }
                                        sales.lstProduct.Add(product);
                                    }

                                }
                            }
                        }

                    }

                }


                responseMessage.ResponseObj = lstSales;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllSales");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllSales");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }
        
        /// <summary>
        /// Get all Sales
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllHoldSales(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<VMSales> lstSales = new List<VMSales>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstSales = await _posDbContext.VMSales.Where(x=> x.Status == (int)Enums.Status.Hold).OrderBy(x => x.CreatedDate).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstSales.Count;

                foreach (VMSales sales in lstSales)
                {
                    Sales objSales = _posDbContext.Sales.AsNoTracking().Where(x => x.SalesCode == sales.SalesCode).FirstOrDefault();
                    if (objSales != null)
                    {
                        List<SalesProductMapping> lstSalesProductMapping = await _posDbContext.SalesProductMapping.Where(x => x.SalesID == objSales.SalesID).ToListAsync();
                        if (lstSalesProductMapping.Count > 0)
                        {
                            List<Product> lstProduct = new List<Product>();

                            lstProduct = await _posDbContext.Product.Where(p =>
                                        lstSalesProductMapping.Select(spm => spm.ProductID).Contains(p.ProductID)).ToListAsync();
                            if (lstProduct.Count > 0)
                            {
                                foreach (Product objProduct in lstProduct)
                                {
                                    VMProduct product = new VMProduct();
                                    product = JsonConvert.DeserializeObject<VMProduct>(JsonConvert.SerializeObject(objProduct));


                                    if (product != null)
                                    {
                                        if (!string.IsNullOrEmpty(product.Image))
                                        {
                                            string getshowurl = _configuration.GetSection("attachments").GetSection("showfilepath").Value;
                                            product.Image = getshowurl + product.Image;
                                        }
                                        sales.lstProduct.Add(product);
                                    }

                                }
                            }
                        }

                    }

                }


                responseMessage.ResponseObj = lstSales;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllHoldSales");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllHoldSales");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Get sales by sales ID
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetSalesById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Sales objSales = new Sales();
                int salesID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objSales = await _posDbContext.Sales.FirstOrDefaultAsync(x => x.SalesID == salesID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objSales;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetSalesById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetSalesById");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Get sales by sales code
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetSalesBySalesCode(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Sales objSales = new Sales();
                List<SalesProductMapping> lstSalesProductMapping = new List<SalesProductMapping>();
                List<Product> lstProduct = new List<Product>();

                var payload = JsonConvert.DeserializeObject<VMSales>(requestMessage?.RequestObj.ToString());
                string salesCode = payload.SalesCode;
                int branchID = payload.BranchID;


                objSales = await _posDbContext.Sales.AsNoTracking().FirstOrDefaultAsync(x => x.SalesCode == salesCode && x.BranchID == branchID);

                if (objSales != null)
                {
                    lstSalesProductMapping = await _posDbContext.SalesProductMapping.Where(x => x.SalesID == objSales.SalesID).ToListAsync();
                    if (lstSalesProductMapping.Count > 0)
                    {
                        //List<int> lstProductIds = lstSalesProductMapping.Select(x => x.ProductID).ToList();

                        lstProduct = await _posDbContext.Product.Where(p =>
                                    lstSalesProductMapping.Select(ppm => ppm.ProductID).Contains(p.ProductID)).ToListAsync();
                        if (lstProduct.Count > 0)
                        {
                            foreach (Product objProduct in lstProduct)
                            {
                                VMProduct product = new VMProduct();
                                product = JsonConvert.DeserializeObject<VMProduct>(JsonConvert.SerializeObject(objProduct));
                                if (product != null)
                                {
                                    product.Qty = lstSalesProductMapping.Where(x => x.ProductID == objProduct.ProductID).FirstOrDefault().Qty;
                                    objSales.lstProduct.Add(product);
                                }

                            }
                        }
                    }

                }
                else
                {
                    responseMessage.Message = "Sales not found";
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    return responseMessage;
                }


                responseMessage.ResponseObj = objSales;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetSalesBySalesCode");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetSalesById");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }


        /// <summary>
        /// Get sales by sales code for view the sales
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetSalesBySalesCodeForView(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                VMSales objSales = new VMSales();
                List<SalesProductMapping> lstSalesProductMapping = new List<SalesProductMapping>();
                List<Product> lstProduct = new List<Product>();

                var payload = JsonConvert.DeserializeObject<VMSales>(requestMessage?.RequestObj.ToString());
                string salesCode = payload.SalesCode;
                int branchID = payload.BranchID;

                objSales = await _posDbContext.VMSales.AsNoTracking().FirstOrDefaultAsync(x => x.SalesCode == salesCode && x.BranchID ==branchID);
                Sales objSalesWithID = await _posDbContext.Sales.AsNoTracking().FirstOrDefaultAsync(x => x.SalesCode == salesCode && x.BranchID == branchID);

                if (objSales != null && objSalesWithID != null)
                {
                    objSales.objCustomer = await _posDbContext.Customer.Where(x => x.CustomerID == objSalesWithID.CustomerID).FirstOrDefaultAsync();

                    lstSalesProductMapping = await _posDbContext.SalesProductMapping.Where(x => x.SalesID == objSalesWithID.SalesID).ToListAsync();
                    if (lstSalesProductMapping.Count > 0)
                    {
                        //List<int> lstProductIds = lstSalesProductMapping.Select(x => x.ProductID).ToList();

                        lstProduct = await _posDbContext.Product.Where(p =>
                                    lstSalesProductMapping.Select(ppm => ppm.ProductID).Contains(p.ProductID)).ToListAsync();
                        if (lstProduct.Count > 0)
                        {
                            foreach (Product objProduct in lstProduct)
                            {
                                VMProduct product = new VMProduct();
                                product = JsonConvert.DeserializeObject<VMProduct>(JsonConvert.SerializeObject(objProduct));
                                if (product != null)
                                {
                                    product.Qty = lstSalesProductMapping.Where(x => x.ProductID == objProduct.ProductID).FirstOrDefault().Qty;
                                    objSales.lstProduct.Add(product);
                                }

                            }
                        }
                    }

                }
                else
                {
                    responseMessage.Message = "Sales not found";
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    return responseMessage;
                }


                responseMessage.ResponseObj = objSales;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetSalesBySalesCodeForView");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetSalesById");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }


        /// <summary>
        /// Delete sales
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> DeleteSales(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                VMSales objSales = JsonConvert.DeserializeObject<VMSales>(requestMessage?.RequestObj.ToString());

                Sales existingSales = await _posDbContext.Sales.AsNoTracking().FirstOrDefaultAsync(x => x.SalesCode == objSales.SalesCode);

                if (existingSales.SalesID > 0)
                {
                    List<SalesProductMapping> lstSalesProductMapping = await _posDbContext.SalesProductMapping.AsNoTracking().Where(x => x.SalesID == existingSales.SalesID).ToListAsync();

                    if (lstSalesProductMapping.Count > 0)
                    {
                        foreach (var item in lstSalesProductMapping)
                        {
                            BranchProductMapping objBranchProductMapping = await _posDbContext.BranchProductMapping.AsNoTracking().Where(x => x.ProductID == item.ProductID && x.BranchID == existingSales.BranchID).FirstOrDefaultAsync();
                            if (objBranchProductMapping != null)
                            {
                                objBranchProductMapping.Quantity = (int)(objBranchProductMapping.Quantity + item.Qty);

                                _posDbContext.BranchProductMapping.Update(objBranchProductMapping);
                            }
                            _posDbContext.SalesProductMapping.Remove(item);
                        }
                    }
                    if (existingSales.PayAmount > 0 && existingSales.AccountID > 0)
                    {

                        List<AccountStatement> exist = await _posDbContext.AccountStatement.AsNoTracking().Where(x => x.SalesID == existingSales.SalesID).ToListAsync();
                        if (exist.Count > 0)
                        {
                            _posDbContext.AccountStatement.RemoveRange(exist);
                        }
                    }
                    _posDbContext.Sales.Remove(existingSales);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Sales not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteSales");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteSales");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Sales
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveSales(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {
                Sales objSales = JsonConvert.DeserializeObject<Sales>(requestMessage?.RequestObj.ToString());
                if (objSales != null)
                {
                    if (CheckedValidation(objSales, responseMessage))
                    {
                        //VMGetAccountBalanceExpense existAccount = await _posDbContext.VMGetAccountBalanceExpense.AsNoTracking().Where(x => x.AccountID == objSales.AccountID).FirstOrDefaultAsync();

                        if (objSales.SalesID > 0)
                        {
                            Sales existingSales = await _posDbContext.Sales.AsNoTracking().FirstOrDefaultAsync(x => x.SalesID == objSales.SalesID);
                            if (existingSales != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                //objSales.DueAmount = objSales.TotalSalesPrice - objSales.PayAmount;
                                objSales.CreatedDate = existingSales.CreatedDate;
                                objSales.CreatedBy = existingSales.CreatedBy;
                                objSales.UpdatedDate = DateTime.Now;
                                objSales.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Sales.Update(objSales);

                                List<SalesProductMapping> existProduct = await _posDbContext.SalesProductMapping.AsNoTracking().Where(x => x.SalesID == objSales.SalesID).ToListAsync();
                                if (existProduct.Count > 0)
                                {
                                    _posDbContext.SalesProductMapping.RemoveRange(existProduct);
                                }

                            }
                        }
                        else
                        {
                            // Get the current timestamp
                            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            // Generate a random number using the timestamp as a seed
                            //var randomNumber = new Random(timestamp.GetHashCode()).Next();
                            objSales.SalesCode = "SL" + timestamp.ToString();

                            //objSales.DueAmount = objSales.TotalSalesPrice - objSales.PayAmount;

                            objSales.CreatedDate = DateTime.Now;
                            objSales.CreatedBy = requestMessage.UserID;
                            await _posDbContext.Sales.AddAsync(objSales);

                        }

                        await _posDbContext.SaveChangesAsync();

                        //list of product added in the DB
                        if (objSales.lstProduct.Count > 0)
                        {
                            foreach (VMProduct product in objSales.lstProduct)
                            {
                                Product existProduct = await _posDbContext.Product.AsNoTracking().Where(x => x.SKU == product.SKU).FirstOrDefaultAsync();

                                if (existProduct != null)
                                {
                                    // update quantity of sale product
                                    if (objSales.Status == (int)Enums.Status.Active)
                                    {

                                        BranchProductMapping existBranchProductMapping = await _posDbContext.BranchProductMapping.AsNoTracking().Where(x => x.ProductID == existProduct.ProductID && x.BranchID == objSales.BranchID).FirstOrDefaultAsync();
                                        if (existBranchProductMapping != null)
                                        {
                                            existBranchProductMapping.Quantity = (int)(existBranchProductMapping.Quantity - (product.Qty ?? 0));

                                            _posDbContext.BranchProductMapping.Update(existBranchProductMapping);
                                        }
                                        else
                                        {
                                            Sales obj = _posDbContext.Sales.AsNoTracking().Where(s => s.SalesID == objSales.SalesID).FirstOrDefault();
                                            _posDbContext.Sales.Remove(obj);
                                            _posDbContext.SaveChanges();

                                            responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                                            responseMessage.Message = "Empty stock! Please check stock before sale";
                                            return responseMessage;
                                        }
                                    }

                                    SalesProductMapping objSalesProductMapping = new SalesProductMapping();
                                    objSalesProductMapping.ProductID = existProduct.ProductID;
                                    objSalesProductMapping.UnitPrice = product.FinalPrice;
                                    objSalesProductMapping.Qty = product.Qty;
                                    objSalesProductMapping.TotalPrice = (product.Qty * product.FinalPrice);
                                    objSalesProductMapping.SalesID = objSales.SalesID;

                                    await _posDbContext.SalesProductMapping.AddAsync(objSalesProductMapping);

                                    //_posDbContext.Product.Update(existProduct);
                                }
                            }
                            await _posDbContext.SaveChangesAsync();
                        }

                        // remove existing payment from the DB
                        AccountStatement existingStatement = await _posDbContext.AccountStatement.AsNoTracking().Where(x => x.SalesID == objSales.SalesID).FirstOrDefaultAsync();
                        if (existingStatement != null)
                        {
                            _posDbContext.Remove(existingStatement);
                        }
                        // payment added in the DB
                        if (objSales.PayAmount > 0 && objSales.AccountID > 0 && objSales.Status == (int)Enums.Status.Active)
                        {
                            AccountStatement objAccountStatement = new AccountStatement();
                            objAccountStatement.SalesID = objSales.SalesID;
                            objAccountStatement.BranchID = objSales.BranchID;
                            objAccountStatement.AccountID = (int)objSales.AccountID;
                            objAccountStatement.InBalance = (double)objSales.PayAmount;
                            objAccountStatement.CreatedDate = DateTime.Now;
                            objAccountStatement.CreatedBy = requestMessage.UserID;

                            await _posDbContext.AccountStatement.AddAsync(objAccountStatement);
                            await _posDbContext.SaveChangesAsync();
                        }


                        responseMessage.ResponseObj = objSales;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveSales");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveSales");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objSales"></param>
        /// <returns></returns>
        private bool CheckedValidation(Sales objSales, ResponseMessage responseMessage)
        {

            bool result = true;
            Sales existingSales = new Sales();

            existingSales = _posDbContext.Sales.Where(x => x.SalesCode == objSales.SalesCode).AsNoTracking().FirstOrDefault();
            if (existingSales != null && objSales.SalesID == 0)
            {
                responseMessage.Message = "Sales code is already exist";
                return false;
            }

            return true;
        }
    }
}
