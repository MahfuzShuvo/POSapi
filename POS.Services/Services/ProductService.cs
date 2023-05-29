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
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using POS.Common.VM;
using static POS.Common.Enums.Enums;

namespace POS.Services
{
    public class ProductService : IProductService
    {
        private readonly POSDbContext _posDbContext;
        private readonly IConfiguration _configuration;

        public ProductService(POSDbContext ctx, IConfiguration configuration)
        {
            _posDbContext = ctx;
            this._configuration = configuration;
        }

        /// <summary>
        /// Get Product count by category
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetProductCountByCategory(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<VMCountProductByCategory> lstVMCountProductByCategory = new List<VMCountProductByCategory>();

                lstVMCountProductByCategory = await _posDbContext.VMCountProductByCategory.ToListAsync();

                responseMessage.ResponseObj = lstVMCountProductByCategory;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetProductCountByCategory");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetProductCountByCategory");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Get all Product
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllProduct(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<VMProduct> lstProduct = new List<VMProduct>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstProduct = await _posDbContext.VMProduct.Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstProduct.Count;

                foreach (VMProduct product in lstProduct)
                {
                    if (!string.IsNullOrEmpty(product.Image))
                    {
                        string getshowUrl = _configuration.GetSection("Attachments").GetSection("ShowFilePath").Value;
                        product.Image = getshowUrl + product.Image;
                    }

                }

                responseMessage.ResponseObj = lstProduct;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllProduct");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllProduct");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Get all Product by categoryID
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllProductByCategoryID(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int categoryID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());
            try
            {
                List<VMProduct> lstProduct = new List<VMProduct>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;
                if (categoryID > 0)
                {
                    lstProduct = await _posDbContext.VMProduct.Where(x => x.CategoryID == categoryID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                }
                else
                {
                    lstProduct = await _posDbContext.VMProduct.Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                }
                
                responseMessage.TotalCount = lstProduct.Count;

                foreach (VMProduct product in lstProduct)
                {
                    if (!string.IsNullOrEmpty(product.Image))
                    {
                        string getshowUrl = _configuration.GetSection("Attachments").GetSection("ShowFilePath").Value;
                        product.Image = getshowUrl + product.Image;
                    }

                }

                responseMessage.ResponseObj = lstProduct;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllProduct");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllProduct");
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
        public async Task<ResponseMessage> GetProductById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Product objProduct = new Product();
                int productID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objProduct = await _posDbContext.Product.FirstOrDefaultAsync(x => x.ProductID == productID && x.Status == (int)Enums.Status.Active);

                string getshowUrl = _configuration.GetSection("Attachments").GetSection("ShowFilePath").Value;
                objProduct.Image = getshowUrl + objProduct.Image;

                responseMessage.ResponseObj = objProduct;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetProductById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetProductById");
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
        public async Task<ResponseMessage> GetProductBySlug(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Product objProduct = new Product();
                
                Product product = JsonConvert.DeserializeObject<Product>(requestMessage?.RequestObj.ToString());
                string slug = product.Slug;

                objProduct = await _posDbContext.Product.FirstOrDefaultAsync(x => x.Slug == slug);
                if (objProduct != null)
                {
                    string getshowUrl = _configuration.GetSection("Attachments").GetSection("ShowFilePath").Value;
                    objProduct.Image = getshowUrl + objProduct.Image;

                    responseMessage.ResponseObj = objProduct;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                }
                else
                {
                    responseMessage.Message = "Product not exist";
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetProductBySlug");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetProductById");
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
        public async Task<ResponseMessage> DeleteProduct(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Product objProduct = JsonConvert.DeserializeObject<Product>(requestMessage?.RequestObj.ToString());

                Product existingProduct = await _posDbContext.Product.AsNoTracking().FirstOrDefaultAsync(x => x.SKU == objProduct.SKU);

                if (existingProduct.ProductID > 0)
                {

                    string saveFilePath = _configuration.GetSection("Attachments").GetSection("SaveFilePath").Value;
                    string showFilePath = _configuration.GetSection("Attachments").GetSection("ShowFilePath").Value;

                    string showUrl = (!string.IsNullOrEmpty(objProduct.Image) && (objProduct.Image != CommonConstant.NoImage)) ? (showFilePath + existingProduct.Image) : String.Empty;
                    //remove image from directory
                    string fileRemovePath = showUrl.Replace(showFilePath, saveFilePath);

                    if (!string.IsNullOrEmpty(fileRemovePath))
                    {
                        //delete file from folder.
                        if (File.Exists(fileRemovePath))
                        {
                            File.Delete(fileRemovePath);
                        }
                    }

                    _posDbContext.Product.Remove(existingProduct);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Product not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteProduct");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteProduct");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        public async Task<ResponseMessage> GetInitialDataForSaveProduct(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Category> lstCategory = await _posDbContext.Category.Where(x => x.Status == (int)Enums.Status.Active).ToListAsync();
                List<Brand> lstBrand = await _posDbContext.Brand.Where(x => x.Status == (int)Enums.Status.Active).ToListAsync();
                List<Unit> lstUnit = await _posDbContext.Unit.Where(x => x.Status == (int)Enums.Status.Active).ToListAsync();


                responseMessage.ResponseObj = new { lstCategory, lstBrand, lstUnit };
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetInitialDataForSaveProduct");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetProductById");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Product
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveProduct(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Product objProduct = JsonConvert.DeserializeObject<Product>(requestMessage?.RequestObj.ToString());

                string saveFilePath = _configuration.GetSection("Attachments").GetSection("SaveFilePath").Value;
                string showFilePath = _configuration.GetSection("Attachments").GetSection("ShowFilePath").Value;

                if (objProduct != null)
                {
                    if (CheckedValidation(objProduct, responseMessage))
                    {
                        string showUrl = (!string.IsNullOrEmpty(objProduct.Image) && (objProduct.Image != CommonConstant.NoImage)) ? (showFilePath + objProduct.Image) : String.Empty;
                        if (!string.IsNullOrEmpty(objProduct.Attachment?.Content))
                        {
                            //remove image from directory
                            string fileRemovePath = showUrl.Replace(showFilePath, saveFilePath);

                            if (!string.IsNullOrEmpty(fileRemovePath))
                            {
                                //delete file from folder.
                                if (File.Exists(fileRemovePath))
                                {
                                    File.Delete(fileRemovePath);
                                    showUrl = String.Empty;
                                }
                            }


                            //save image in the directory
                            string[] base64image = objProduct.Attachment?.Content.Split(',');

                            string fileName = objProduct.Attachment?.Name + "_" + DateTime.Now.ToString("MM_dd_yyyy_hhss") + "." + objProduct.Attachment?.Extension;

                            string filePath = saveFilePath + fileName;
                            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(base64image[1]));

                            showUrl = filePath.Replace(saveFilePath, showFilePath);

                            objProduct.Image = fileName;
                        }
                        else
                        {
                            objProduct.Image = CommonConstant.NoImage;
                        }


                        if (!string.IsNullOrEmpty(objProduct.SKU))
                        {
                            Product existingProduct = await this._posDbContext.Product.AsNoTracking().FirstOrDefaultAsync(x => x.SKU == objProduct.SKU);
                            if (existingProduct != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objProduct.CreatedDate = existingProduct.CreatedDate;
                                objProduct.CreatedBy = existingProduct.CreatedBy;
                                objProduct.UpdatedDate = DateTime.Now;
                                objProduct.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Product.Update(objProduct);

                            }
                        }
                        else
                        {
                            
                            //objProduct.Status = (int)Enums.Status.Active;
                            objProduct.CreatedDate = DateTime.Now;
                            objProduct.CreatedBy = requestMessage.UserID;

                            objProduct.SKU = "GP"+DateTime.Now.ToString("MMddyyyyhhmm");
                            objProduct.Slug = GenerateSlug(objProduct.ProductName) + "-" + objProduct.SKU;

                            await _posDbContext.Product.AddAsync(objProduct);

                        }


                        await _posDbContext.SaveChangesAsync();

                        objProduct.Image = (!string.IsNullOrEmpty(objProduct.Image)) ? (showFilePath + objProduct.Image) : "";
                        responseMessage.ResponseObj = objProduct;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveProduct");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveProduct");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        // Generates SKU
        public string GenerateSKU(string productName, int categoryID, DateTime productCreated)
        {
            // Extract first 3 letters of product name
            string nameCode = productName.Substring(0, Math.Min(productName.Length, 3)).ToUpper();

            // Extract last 2 digits of product ID
            string idCode = (categoryID % 100).ToString("D2");

            // Extract last 4 digits of product creation year
            string yearCode = (productCreated.Year % 10000).ToString("D4").Substring(2);

            // Extract product creation month and day
            string monthDayCode = productCreated.ToString("MMdd");

            // Combine all codes to generate SKU
            string skuCode = nameCode + idCode + yearCode + monthDayCode;

            return skuCode;
        }

        //generate slug
        public string GenerateSlug(string phrase)
        {
            string str = phrase.ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            //str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public async Task<ResponseMessage> ChangeProductStatus(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Update;
            try
            {
                Product objProduct = JsonConvert.DeserializeObject<Product>(requestMessage?.RequestObj.ToString());
                if (objProduct != null)
                {
                    Product existingProduct = await _posDbContext.Product.Where(x => x.SKU == objProduct.SKU).FirstOrDefaultAsync();
                    if (existingProduct.ProductID > 0)
                    {
                        existingProduct.Status = objProduct.Status;
                        existingProduct.UpdatedDate = DateTime.Now;
                        existingProduct.UpdatedBy = requestMessage.UserID;

                        _posDbContext.Product.Update(existingProduct);

                        await _posDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = objProduct;
                        responseMessage.Message = "Product status changed successfully";
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "ChangeProductStatus");

                    }
                    else
                    {
                        responseMessage.Message = "Product not found in DB";
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    }
                }
                else
                {
                    responseMessage.Message = "Something went wrong";
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "ChangeProductStatus");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }
            return responseMessage;
        }


        public async Task<ResponseMessage> SearchProduct(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.View;
            try
            {
                List<VMProduct> lstProduct = new List<VMProduct>();
                string searchText = requestMessage.RequestObj.ToString();

                lstProduct = await _posDbContext.VMProduct.Where(x => x.Status == (int)Enums.Status.Active).ToListAsync();

                lstProduct = lstProduct.Where(x => x.ProductName.ToLower().Contains(searchText?.ToLower()) 
                                                || x.SKU.ToLower().Contains(searchText?.ToLower())).ToList();

                if (lstProduct.Count > 0)
                {
                    responseMessage.ResponseObj = lstProduct;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                    //Log write
                    LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SearchProduct");
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Warning;
                    responseMessage.Message = "No product found";
                }
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SearchProduct");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }
            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objProduct"></param>
        /// <returns></returns>
        private bool CheckedValidation(Product objProduct, ResponseMessage responseMessage)
        {
            bool result = true;
            Product existingProduct = new Product();


            if (String.IsNullOrEmpty(objProduct.ProductName))
            {
                responseMessage.Message = "Product name is required";
                return false;
            }

            return true;
        }
    }
}
