using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Branch")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _BranchService;

        public BranchController(IBranchService BranchService)
        {
            _BranchService = BranchService;
        }

        [HttpPost("GetAllBranch")]
        public async Task<ResponseMessage> GetAllBranch(RequestMessage requestMessage)
        {
            return await _BranchService.GetAllBranch(requestMessage);
        }

        [HttpPost("GetBranchById")]
        public async Task<ResponseMessage> GetBranchById(RequestMessage requestMessage)
        {
            return await _BranchService.GetBranchById(requestMessage);
        }

        [HttpPost("SaveBranch")]
        public async Task<ResponseMessage> SaveBranchs(RequestMessage requestMessage)
        {
            return await _BranchService.SaveBranch(requestMessage);
        }

        [HttpPost("DeleteBranch")]
        public async Task<ResponseMessage> DeleteBranch(RequestMessage requestMessage)
        {
            return await _BranchService.DeleteBranch(requestMessage);
        }

        [HttpPost("AssignUserToBranch")]
        public async Task<ResponseMessage> AssignUserToBranch(RequestMessage requestMessage)
        {
            return await _BranchService.AssignUserToBranch(requestMessage);
        } 
        
        [HttpPost("RemoveUserFromBranch")]
        public async Task<ResponseMessage> RemoveUserFromBranch(RequestMessage requestMessage)
        {
            return await _BranchService.RemoveUserFromBranch(requestMessage);
        }
    }
}
