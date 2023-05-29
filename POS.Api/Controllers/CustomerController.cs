using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("GetAllCustomer")]
        public async Task<ResponseMessage> GetAllCustomer(RequestMessage requestMessage)
        {
            return await _customerService.GetAllCustomer(requestMessage);
        }

        [HttpPost("GetCustomerById")]
        public async Task<ResponseMessage> GetCustomerById(RequestMessage requestMessage)
        {
            return await _customerService.GetCustomerById(requestMessage);
        }

        [HttpPost("SaveCustomer")]
        public async Task<ResponseMessage> SaveCustomers(RequestMessage requestMessage)
        {
            return await _customerService.SaveCustomer(requestMessage);
        }

        [HttpPost("DeleteCustomer")]
        public async Task<ResponseMessage> DeleteCustomer(RequestMessage requestMessage)
        {
            return await _customerService.DeleteCustomer(requestMessage);
        }
    }
}
