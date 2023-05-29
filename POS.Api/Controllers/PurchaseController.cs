using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Purchase")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpPost("GetAllPurchase")]
        public async Task<ResponseMessage> GetAllPurchase(RequestMessage requestMessage)
        {
            return await _purchaseService.GetAllPurchase(requestMessage);
        }

        [HttpPost("GetPurchaseById")]
        public async Task<ResponseMessage> GetPurchaseById(RequestMessage requestMessage)
        {
            return await _purchaseService.GetPurchaseById(requestMessage);
        }
        
        [HttpPost("GetPurchaseByPurchaseCode")]
        public async Task<ResponseMessage> GetPurchaseByPurchaseCode(RequestMessage requestMessage)
        {
            return await _purchaseService.GetPurchaseByPurchaseCode(requestMessage);
        }

        [HttpPost("SavePurchase")]
        public async Task<ResponseMessage> SavePurchases(RequestMessage requestMessage)
        {
            return await _purchaseService.SavePurchase(requestMessage);
        }

        [HttpPost("DeletePurchase")]
        public async Task<ResponseMessage> DeletePurchase(RequestMessage requestMessage)
        {
            return await _purchaseService.DeletePurchase(requestMessage);
        }
    }
}
