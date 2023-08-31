using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Sales")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;

        public SalesController(ISalesService salesService)
        {
            _salesService = salesService;
        }

        [HttpPost("GetAllSales")]
        public async Task<ResponseMessage> GetAllSales(RequestMessage requestMessage)
        {
            return await _salesService.GetAllSales(requestMessage);
        }

        [HttpPost("GetSalesById")]
        public async Task<ResponseMessage> GetSalesById(RequestMessage requestMessage)
        {
            return await _salesService.GetSalesById(requestMessage);
        }

        [HttpPost("GetSalesBySalesCode")]
        public async Task<ResponseMessage> GetSalesBySalesCode(RequestMessage requestMessage)
        {
            return await _salesService.GetSalesBySalesCode(requestMessage);
        }

        [HttpPost("GetSalesBySalesCodeForView")]
        public async Task<ResponseMessage> GetSalesBySalesCodeForView(RequestMessage requestMessage)
        {
            return await _salesService.GetSalesBySalesCodeForView(requestMessage);
        }

        [HttpPost("SaveSales")]
        public async Task<ResponseMessage> SaveSales(RequestMessage requestMessage)
        {
            return await _salesService.SaveSales(requestMessage);
        }

        [HttpPost("DeleteSales")]
        public async Task<ResponseMessage> DeleteSales(RequestMessage requestMessage)
        {
            return await _salesService.DeleteSales(requestMessage);
        }
    }
}
