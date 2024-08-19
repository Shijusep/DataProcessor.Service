using DataProcessor.Service.CQRS.Commands;
using MediatR; 
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DataProcessor.Service.Controllers;
public class ExpenseController : ApiControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ExpenseController> _logger;
    public ExpenseController(IMediator mediator, ILogger<ExpenseController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("process")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ProcessExpense([FromBody] string inputText)
    {
        _logger.LogInformation("ProcessExpense started with inputText: {InputText}", inputText);

        try
        {
            var command = new ExtractExpenseDataCommand { InputText = inputText };
            var result = await _mediator.Send(command);

            if (result.Error != null)
            {
                _logger.LogWarning("Processing failed with error: {Error}", result.Error.Message);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("ProcessExpense completed successfully for inputText: {InputText}", inputText);
            return Ok(result.Expense);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing the expense.");
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }
}
