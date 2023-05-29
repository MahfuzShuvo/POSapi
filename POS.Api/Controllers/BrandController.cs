using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Brand")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpPost("GetAllBrand")]
        public async Task<ResponseMessage> GetAllBrand(RequestMessage requestMessage)
        {
            return await _brandService.GetAllBrand(requestMessage);
        }

        [HttpPost("GetBrandById")]
        public async Task<ResponseMessage> GetBrandById(RequestMessage requestMessage)
        {
            return await _brandService.GetBrandById(requestMessage);
        }

        [HttpPost("SaveBrand")]
        public async Task<ResponseMessage> SaveBrands(RequestMessage requestMessage)
        {
            return await _brandService.SaveBrand(requestMessage);
        }

        [HttpPost("DeleteBrand")]
        public async Task<ResponseMessage> DeleteBrand(RequestMessage requestMessage)
        {
            return await _brandService.DeleteBrand(requestMessage);
        }
    }
}
