using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Unit")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpPost("GetAllUnit")]
        public async Task<ResponseMessage> GetAllUnit(RequestMessage requestMessage)
        {
            return await _unitService.GetAllUnit(requestMessage);
        }

        [HttpPost("GetUnitById")]
        public async Task<ResponseMessage> GetUnitById(RequestMessage requestMessage)
        {
            return await _unitService.GetUnitById(requestMessage);
        }

        [HttpPost("SaveUnit")]
        public async Task<ResponseMessage> SaveUnits(RequestMessage requestMessage)
        {
            return await _unitService.SaveUnit(requestMessage);
        }

        [HttpPost("DeleteUnit")]
        public async Task<ResponseMessage> DeleteUnit(RequestMessage requestMessage)
        {
            return await _unitService.DeleteUnit(requestMessage);
        }
    }
}
