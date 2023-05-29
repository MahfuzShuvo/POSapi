using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("GetAllCategory")]
        public async Task<ResponseMessage> GetAllCategory(RequestMessage requestMessage)
        {
            return await _categoryService.GetAllCategory(requestMessage);
        }

        [HttpPost("GetCategoryById")]
        public async Task<ResponseMessage> GetCategoryById(RequestMessage requestMessage)
        {
            return await _categoryService.GetCategoryById(requestMessage);
        }

        [HttpPost("SaveCategory")]
        public async Task<ResponseMessage> SaveCategorys(RequestMessage requestMessage)
        {
            return await _categoryService.SaveCategory(requestMessage);
        }

        [HttpPost("DeleteCategory")]
        public async Task<ResponseMessage> DeleteCategory(RequestMessage requestMessage)
        {
            return await _categoryService.DeleteCategory(requestMessage);
        }
    }
}
