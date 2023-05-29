using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("GetAllAccount")]
        public async Task<ResponseMessage> GetAllAccount(RequestMessage requestMessage)
        {
            return await _accountService.GetAllAccount(requestMessage);
        }

        [HttpPost("GetAccountById")]
        public async Task<ResponseMessage> GetAccountById(RequestMessage requestMessage)
        {
            return await _accountService.GetAccountById(requestMessage);
        }

        [HttpPost("SaveAccount")]
        public async Task<ResponseMessage> SaveAccounts(RequestMessage requestMessage)
        {
            return await _accountService.SaveAccount(requestMessage);
        }

        [HttpPost("DeleteAccount")]
        public async Task<ResponseMessage> DeleteAccount(RequestMessage requestMessage)
        {
            return await _accountService.DeleteAccount(requestMessage);
        }

        [HttpPost("AddBalance")]
        public async Task<ResponseMessage> AddBalance(RequestMessage requestMessage)
        {
            return await _accountService.AddBalance(requestMessage);
        }

        [HttpPost("ShowAccountStatement")]
        public async Task<ResponseMessage> ShowAccountStatement(RequestMessage requestMessage)
        {
            return await _accountService.ShowAccountStatement(requestMessage);
        }
    }
}
