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

namespace POS.Services
{
    public class BrandService : IBrandService
    {
        private readonly POSDbContext _posDbContext;
        private readonly IConfiguration _configuration;
        public BrandService(POSDbContext ctx, IConfiguration configuration)
        {
            _posDbContext = ctx;
            this._configuration = configuration;
        }

        /// <summary>
        /// Get all Brand
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllBrand(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Brand> lstBrand = new List<Brand>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstBrand = await _posDbContext.Brand.OrderBy(x => x.BrandID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstBrand.Count;

                foreach (Brand brand in lstBrand)
                {
                    if (!string.IsNullOrEmpty(brand.Logo))
                    {
                        string getshowUrl = _configuration.GetSection("Attachments").GetSection("ShowFilePath").Value;
                        brand.Logo = getshowUrl + brand.Logo;
                    }
                }

                responseMessage.ResponseObj = lstBrand;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllBrand");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllBrand");
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
        public async Task<ResponseMessage> GetBrandById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Brand objBrand = new Brand();
                int brandID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objBrand = await _posDbContext.Brand.FirstOrDefaultAsync(x => x.BrandID == brandID && x.Status == (int)Enums.Status.Active);

                string getshowUrl = _configuration.GetSection("Attachments").GetSection("ShowFilePath").Value;
                objBrand.Logo = getshowUrl + objBrand.Logo;

                responseMessage.ResponseObj = objBrand;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetBrandById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetBrandById");
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
        public async Task<ResponseMessage> DeleteBrand(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Brand objBrand = new Brand();
                int brandID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objBrand = await _posDbContext.Brand.AsNoTracking().FirstOrDefaultAsync(x => x.BrandID == brandID);

                if (objBrand.BrandID > 0)
                {
                    string saveFilePath = _configuration.GetSection("Attachments").GetSection("SaveFilePath").Value;
                    string showFilePath = _configuration.GetSection("Attachments").GetSection("ShowFilePath").Value;

                    string showUrl = !string.IsNullOrEmpty(objBrand.Logo) ? (showFilePath + objBrand.Logo) : String.Empty;
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

                    _posDbContext.Brand.Remove(objBrand);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Brand not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteBrand");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteBrand");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Brand
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveBrand(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            int actionType = (int)Enums.ActionType.Insert;

            try
            {
                Brand objBrand = JsonConvert.DeserializeObject<Brand>(requestMessage?.RequestObj.ToString());

                string saveFilePath = _configuration.GetSection("Attachments").GetSection("SaveFilePath").Value;
                string showFilePath = _configuration.GetSection("Attachments").GetSection("ShowFilePath").Value;

                if (objBrand != null)
                {
                    if (CheckedValidation(objBrand, responseMessage))
                    {

                        if (!string.IsNullOrEmpty(objBrand.LogoAttachment?.Content))
                        {
                        string showUrl = !string.IsNullOrEmpty(objBrand.Logo) ? (showFilePath + objBrand.Logo) : String.Empty;
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
                            string[] base64image = objBrand.LogoAttachment?.Content.Split(',');

                            string fileName = objBrand.LogoAttachment?.Name + "_" + DateTime.Now.ToString("MM_dd_yyyy_hhss") + "." + objBrand.LogoAttachment?.Extension;

                            string filePath = saveFilePath + fileName;
                            System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(base64image[1]));

                            showUrl = filePath.Replace(saveFilePath, showFilePath);

                            objBrand.Logo = fileName;
                        }
                        else
                        {
                            objBrand.Logo = !string.IsNullOrEmpty(objBrand.Logo) ? objBrand.Logo : CommonConstant.NoImage;
                        }

                        if (objBrand.BrandID > 0)
                        {
                            Brand existingBrand = await this._posDbContext.Brand.AsNoTracking().FirstOrDefaultAsync(x => x.BrandID == objBrand.BrandID);
                            if (existingBrand != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objBrand.CreatedDate = existingBrand.CreatedDate;
                                objBrand.CreatedBy = existingBrand.CreatedBy;
                                objBrand.UpdatedDate = DateTime.Now;
                                objBrand.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Brand.Update(objBrand);

                            }
                        }
                        else
                        {
                            objBrand.CreatedDate = DateTime.Now;
                            objBrand.CreatedBy = requestMessage.UserID;
                            await _posDbContext.Brand.AddAsync(objBrand);

                        }

                        await _posDbContext.SaveChangesAsync();

                        objBrand.Logo = (!string.IsNullOrEmpty(objBrand.Logo)) ? (showFilePath + objBrand.Logo) : "";
                        responseMessage.ResponseObj = objBrand;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveBrand");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveBrand");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objBrand"></param>
        /// <returns></returns>
        private bool CheckedValidation(Brand objBrand, ResponseMessage responseMessage)
        {

            bool result = true;
            Brand existingBrand = new Brand();


            if (String.IsNullOrEmpty(objBrand.BrandName))
            {
                responseMessage.Message = "Brand name is required";
                return false;
            }
            return true;
        }
    }
}
