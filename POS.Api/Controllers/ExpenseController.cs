using POS.Common.DTO;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers
{
    [Route("api/Expense")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost("GetAllExpense")]
        public async Task<ResponseMessage> GetAllExpense(RequestMessage requestMessage)
        {
            return await _expenseService.GetAllExpense(requestMessage);
        }

        [HttpPost("GetExpenseById")]
        public async Task<ResponseMessage> GetExpenseById(RequestMessage requestMessage)
        {
            return await _expenseService.GetExpenseById(requestMessage);
        }

        [HttpPost("SaveExpense")]
        public async Task<ResponseMessage> SaveExpenses(RequestMessage requestMessage)
        {
            return await _expenseService.SaveExpense(requestMessage);
        }

        [HttpPost("DeleteExpense")]
        public async Task<ResponseMessage> DeleteExpense(RequestMessage requestMessage)
        {
            return await _expenseService.DeleteExpense(requestMessage);
        }
    }
}
