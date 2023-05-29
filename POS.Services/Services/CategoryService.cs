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
    public class CategoryService: ICategoryService
    {
        private readonly POSDbContext _posDbContext;

        public CategoryService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Category
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllCategory(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Category> lstCategory = new List<Category>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstCategory = await _posDbContext.Category.OrderBy(x => x.CategoryID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstCategory.Count;


                responseMessage.ResponseObj = lstCategory;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllCategory");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllCategory");
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
        public async Task<ResponseMessage> GetCategoryById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Category objCategory = new Category();
                int categoryID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objCategory = await _posDbContext.Category.FirstOrDefaultAsync(x => x.CategoryID == categoryID && x.Status == (int)Enums.Status.Active);
                responseMessage.ResponseObj = objCategory;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetCategoryById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetCategoryById");
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
        public async Task<ResponseMessage> DeleteCategory(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Category objCategory = new Category();
                int categoryID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objCategory = await _posDbContext.Category.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryID == categoryID);
               
                if (objCategory.CategoryID > 0)
                {

                    _posDbContext.Category.Remove(objCategory);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseObj = null;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Category not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteCategory");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteCategory");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Category
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveCategory(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Category objCategory = JsonConvert.DeserializeObject<Category>(requestMessage?.RequestObj.ToString());
                if (objCategory != null)
                {
                    if (CheckedValidation(objCategory, responseMessage))
                    {
                        if (objCategory.CategoryID > 0)
                        {
                            Category existingCategory = await this._posDbContext.Category.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryID == objCategory.CategoryID);
                            if (existingCategory != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objCategory.CreatedDate = existingCategory.CreatedDate;
                                objCategory.CreatedBy = existingCategory.CreatedBy;
                                objCategory.UpdatedDate = DateTime.Now;
                                objCategory.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Category.Update(objCategory);

                            }
                        }
                        else
                        {
                            //objCategory.Status = (int)Enums.Status.Active;
                            objCategory.CreatedDate = DateTime.Now;
                            objCategory.CreatedBy = requestMessage.UserID;
                            await _posDbContext.Category.AddAsync(objCategory);

                        }

                     
                        await _posDbContext.SaveChangesAsync();

                        responseMessage.ResponseObj = objCategory;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveCategory");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveCategory");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objCategory"></param>
        /// <returns></returns>
        private bool CheckedValidation(Category objCategory, ResponseMessage responseMessage)
        {

            bool result = true;
            Category existingCategory = new Category();


            if (String.IsNullOrEmpty(objCategory.CategoryName))
            {
                responseMessage.Message = "Category name is required";
                return false;
            }
            return true;
        }
    }
}
