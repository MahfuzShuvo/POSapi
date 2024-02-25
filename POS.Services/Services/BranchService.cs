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
    public class BranchService : IBranchService
    {
        private readonly POSDbContext _posDbContext;

        public BranchService(POSDbContext ctx)
        {
            _posDbContext = ctx;
        }

        /// <summary>
        /// Get all Branch
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllBranch(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Branch> lstBranch = new List<Branch>();
                int totalSkip = 0;
                totalSkip = (requestMessage.PageNumber > 0) ? requestMessage.PageNumber * requestMessage.PageRecordSize : 0;

                lstBranch = await _posDbContext.Branch.OrderBy(x => x.BranchID).Skip(totalSkip).Take(requestMessage.PageRecordSize).ToListAsync();
                responseMessage.TotalCount = lstBranch.Count;

                foreach (Branch branch in lstBranch)
                {
                    List<int?> branchUserMappingIds = new List<int?>();

                    branchUserMappingIds = _posDbContext.BranchUserMapping
                            .AsNoTracking()
                            .Where(x => x.BranchID == branch.BranchID)
                            .Select(x => x.SystemUserID)
                            .ToList();

                    branch.lstAssignedUser = _posDbContext.SystemUser
                            .AsNoTracking()
                            .Where(x => branchUserMappingIds!.Contains(x.SystemUserID)).ToList();
                }


                responseMessage.ResponseObj = lstBranch;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllBranch");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllBranch");
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
        public async Task<ResponseMessage> GetBranchById(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Branch objBranch = new Branch();
                int BranchID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objBranch = await _posDbContext.Branch.FirstOrDefaultAsync(x => x.BranchID == BranchID && x.Status == (int)Enums.Status.Active);

                List<int?> branchUserMappingIds = new List<int?>();

                branchUserMappingIds = _posDbContext.BranchUserMapping
                        .AsNoTracking()
                        .Where(x => x.BranchID == objBranch!.BranchID)
                        .Select(x => x.SystemUserID)
                        .ToList();

                objBranch!.lstAssignedUser = _posDbContext.SystemUser
                        .AsNoTracking()
                        .Where(x => branchUserMappingIds!.Contains(x.SystemUserID)).ToList();

                responseMessage.ResponseObj = objBranch;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetBranchById");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetBranchById");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Get all the Branch by user id
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseMessage> GetAllBranchByUserID(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                List<Branch> lstBranch = new List<Branch>();
                int roleId = _posDbContext.SystemUser.AsNoTracking().Where(x => x.SystemUserID == requestMessage.UserID).Select(x => x.RoleID).FirstOrDefault();

                lstBranch = await _posDbContext.Branch.AsNoTracking().Where(x => x.Status == (int)Enums.Status.Active).ToListAsync();
                if (roleId != 1 && roleId != 2)
                {

                    List<int?> branchUserMappingIds = new List<int?>();

                    branchUserMappingIds = _posDbContext.BranchUserMapping
                            .AsNoTracking()
                            .Where(x => x.SystemUserID == requestMessage.UserID)
                            .Select(x => x.BranchID)
                            .ToList();

                    lstBranch = lstBranch.Where(x => branchUserMappingIds!.Contains(x.BranchID)).ToList();
                }


                responseMessage.ResponseObj = lstBranch;
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.View, requestMessage.UserID, "GetAllBranchByUserID");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.View, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "GetAllBranchByUserID");
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
        public async Task<ResponseMessage> DeleteBranch(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                Branch objBranch = new Branch();
                int BranchID = JsonConvert.DeserializeObject<int>(requestMessage?.RequestObj.ToString());

                objBranch = await _posDbContext.Branch.AsNoTracking().FirstOrDefaultAsync(x => x.BranchID == BranchID);

                if (objBranch.BranchID > 0)
                {
                    List<BranchUserMapping> lstBranchUserMapping = new List<BranchUserMapping>();
                    lstBranchUserMapping = _posDbContext.BranchUserMapping.AsNoTracking().Where(x => x.BranchID == objBranch.BranchID).ToList();
                    _posDbContext.BranchUserMapping.RemoveRange(lstBranchUserMapping);

                    _posDbContext.Branch.Remove(objBranch);

                    await _posDbContext.SaveChangesAsync();

                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.DeleteSuccess;
                }
                else
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Branch not found";
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "DeleteBranch");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "DeleteBranch");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Save and update Branch
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> SaveBranch(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();

            int actionType = (int)Enums.ActionType.Insert;
            try
            {

                Branch objBranch = JsonConvert.DeserializeObject<Branch>(requestMessage?.RequestObj.ToString());
                if (objBranch != null)
                {
                    if (CheckedValidation(objBranch, responseMessage))
                    {
                        if (objBranch.BranchID > 0)
                        {
                            Branch existingBranch = await this._posDbContext.Branch.AsNoTracking().FirstOrDefaultAsync(x => x.BranchID == objBranch.BranchID);
                            if (existingBranch != null)
                            {
                                actionType = (int)Enums.ActionType.Update;
                                objBranch.CreatedDate = existingBranch.CreatedDate;
                                objBranch.CreatedBy = existingBranch.CreatedBy;
                                objBranch.UpdatedDate = DateTime.Now;
                                objBranch.UpdatedBy = requestMessage.UserID;
                                _posDbContext.Branch.Update(objBranch);

                            }
                        }
                        else
                        {
                            objBranch.CreatedDate = DateTime.Now;
                            objBranch.CreatedBy = requestMessage.UserID;
                            await _posDbContext.Branch.AddAsync(objBranch);

                        }

                        await _posDbContext.SaveChangesAsync();

                        // BranchUserMapping update
                        BranchUserMapping existBranchUserMapping = _posDbContext.BranchUserMapping.AsNoTracking().Where(x => x.BranchID == objBranch.BranchID && x.SystemUserID == objBranch.BranchManagerID).FirstOrDefault();

                        if (existBranchUserMapping != null)
                        {
                            _posDbContext.BranchUserMapping.Remove(existBranchUserMapping);
                        }

                        BranchUserMapping objBranchUserMapping = new BranchUserMapping();
                        objBranchUserMapping.SystemUserID = objBranch.BranchManagerID;
                        objBranchUserMapping.BranchID = objBranch.BranchID;
                        objBranchUserMapping.IsManager = true;

                        _posDbContext.BranchUserMapping.Add(objBranchUserMapping);
                        await _posDbContext.SaveChangesAsync();


                        List<int?> branchUserMappingIds = new List<int?>();

                        branchUserMappingIds = _posDbContext.BranchUserMapping
                                .AsNoTracking()
                                .Where(x => x.BranchID == objBranch.BranchID)
                                .Select(x => x.SystemUserID)
                                .ToList();

                        objBranch.lstAssignedUser = _posDbContext.SystemUser
                                .AsNoTracking()
                                .Where(x => branchUserMappingIds!.Contains(x.SystemUserID)).ToList();


                        responseMessage.ResponseObj = objBranch;
                        responseMessage.Message = MessageConstant.SavedSuccessfully;
                        responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;

                        //Log write
                        LogHelper.WriteLog(requestMessage.RequestObj, actionType, requestMessage.UserID, "SaveBranch");
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
                responseMessage.Message = ExceptionHelper.ProcessException(ex, actionType, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "SaveBranch");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;

            }

            return responseMessage;
        }

        /// <summary>
        /// Assign user to the bBranch
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> AssignUserToBranch(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                if (requestMessage?.RequestObj == null)
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Invalid request";
                    return responseMessage;
                }

                BranchUserMapping objBranchUserMapping = JsonConvert.DeserializeObject<BranchUserMapping>(requestMessage?.RequestObj.ToString());

                if (objBranchUserMapping != null)
                {
                    BranchUserMapping exist = _posDbContext.BranchUserMapping.AsNoTracking().Where(x => x.BranchID == objBranchUserMapping.BranchID && x.SystemUserID == objBranchUserMapping.SystemUserID).FirstOrDefault();
                    if (exist == null)
                    {
                        await _posDbContext.AddAsync(objBranchUserMapping);
                        await _posDbContext.SaveChangesAsync();
                    }
                    responseMessage.ResponseObj = objBranchUserMapping;
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = MessageConstant.SavedSuccessfully;
                }

                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Insert, requestMessage.UserID, "AssignUserToBranch");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Insert, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "AssignUserToBranch");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// Remove user from the bBranch
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseMessage> RemoveUserFromBranch(RequestMessage requestMessage)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            try
            {
                if (requestMessage?.RequestObj == null)
                {
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
                    responseMessage.Message = "Invalid request";
                    return responseMessage;
                }

                BranchUserMapping objBranchUserMapping = JsonConvert.DeserializeObject<BranchUserMapping>(requestMessage?.RequestObj.ToString());

                if (objBranchUserMapping != null)
                {
                    BranchUserMapping exist = _posDbContext.BranchUserMapping.AsNoTracking().Where(x => x.BranchID == objBranchUserMapping.BranchID && x.SystemUserID == objBranchUserMapping.SystemUserID).FirstOrDefault();
                    if (exist != null)
                    {
                        _posDbContext.Remove(exist);
                        await _posDbContext.SaveChangesAsync();
                    }
                    responseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                    responseMessage.Message = "Removed Successfully";
                }


                //Log write
                LogHelper.WriteLog(requestMessage?.RequestObj, (int)Enums.ActionType.Delete, requestMessage.UserID, "RemoveUserFromBranch");
            }
            catch (Exception ex)
            {
                //Process excetion, Development mode show real exception and production mode will show custom exception.
                responseMessage.Message = ExceptionHelper.ProcessException(ex, (int)Enums.ActionType.Delete, requestMessage.UserID, JsonConvert.SerializeObject(requestMessage.RequestObj), "RemoveUserFromBranch");
                responseMessage.ResponseCode = (int)Enums.ResponseCode.Failed;
            }

            return responseMessage;
        }

        /// <summary>
        /// validation check
        /// </summary>
        /// <param name="objBranch"></param>
        /// <returns></returns>
        private bool CheckedValidation(Branch objBranch, ResponseMessage responseMessage)
        {

            bool result = true;
            Branch existingBranch = new Branch();


            if (String.IsNullOrEmpty(objBranch.BranchName))
            {
                responseMessage.Message = "Branch name required";
                return false;
            }

            return true;
        }
    }
}
