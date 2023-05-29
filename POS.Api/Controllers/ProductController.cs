using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("GetAllProduct")]
        public async Task<ResponseMessage> GetAllProduct(RequestMessage requestMessage)
        {
            return await _productService.GetAllProduct(requestMessage);
        } 
        
        [HttpPost("GetProductCountByCategory")]
        public async Task<ResponseMessage> GetProductCountByCategory(RequestMessage requestMessage)
        {
            return await _productService.GetProductCountByCategory(requestMessage);
        } 
        
        [HttpPost("GetAllProductByCategoryID")]
        public async Task<ResponseMessage> GetAllProductByCategoryID(RequestMessage requestMessage)
        {
            return await _productService.GetAllProductByCategoryID(requestMessage);
        }

        [HttpPost("GetProductById")]
        public async Task<ResponseMessage> GetProductById(RequestMessage requestMessage)
        {
            return await _productService.GetProductById(requestMessage);
        } 
        
        [HttpPost("GetProductBySlug")]
        public async Task<ResponseMessage> GetProductBySlug(RequestMessage requestMessage)
        {
            return await _productService.GetProductBySlug(requestMessage);
        }

        [HttpPost("GetInitialDataForSaveProduct")]
        public async Task<ResponseMessage> GetInitialDataForSaveProduct(RequestMessage requestMessage)
        {
            return await _productService.GetInitialDataForSaveProduct(requestMessage);
        }

        [HttpPost("SaveProduct")]
        public async Task<ResponseMessage> SaveProducts(RequestMessage requestMessage)
        {
            return await _productService.SaveProduct(requestMessage);
        } 
        
        [HttpPost("ChangeProductStatus")]
        public async Task<ResponseMessage> ChangeProductStatus(RequestMessage requestMessage)
        {
            return await _productService.ChangeProductStatus(requestMessage);
        }

        [HttpPost("DeleteProduct")]
        public async Task<ResponseMessage> DeleteProduct(RequestMessage requestMessage)
        {
            return await _productService.DeleteProduct(requestMessage);
        }
        
        [HttpPost("SearchProduct")]
        public async Task<ResponseMessage> SearchProduct(RequestMessage requestMessage)
        {
            return await _productService.SearchProduct(requestMessage);
        }
    }
}
