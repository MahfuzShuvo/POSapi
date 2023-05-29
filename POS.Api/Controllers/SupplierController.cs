using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Supplier")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpPost("GetAllSupplier")]
        public async Task<ResponseMessage> GetAllSupplier(RequestMessage requestMessage)
        {
            return await _supplierService.GetAllSupplier(requestMessage);
        }

        [HttpPost("GetSupplierById")]
        public async Task<ResponseMessage> GetSupplierById(RequestMessage requestMessage)
        {
            return await _supplierService.GetSupplierById(requestMessage);
        }

        [HttpPost("SaveSupplier")]
        public async Task<ResponseMessage> SaveSuppliers(RequestMessage requestMessage)
        {
            return await _supplierService.SaveSupplier(requestMessage);
        }

        [HttpPost("DeleteSupplier")]
        public async Task<ResponseMessage> DeleteSupplier(RequestMessage requestMessage)
        {
            return await _supplierService.DeleteSupplier(requestMessage);
        }
    }
}
