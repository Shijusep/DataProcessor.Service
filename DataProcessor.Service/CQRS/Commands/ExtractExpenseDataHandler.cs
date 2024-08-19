using DataProcessor.Service.Interface;
using DataProcessor.Service.Models;
using MediatR;
using System.Xml;

namespace DataProcessor.Service.CQRS.Commands;

public class ExtractExpenseDataHandler : IRequestHandler<ExtractExpenseDataCommand, (ExpenseModel Expense, ErrorResponse Error)>
{
    private readonly IExpenseService _expenseService;

    public ExtractExpenseDataHandler(IExpenseService _expenseService)=> this._expenseService = _expenseService;
   

    public async Task<(ExpenseModel Expense, ErrorResponse Error)> Handle(ExtractExpenseDataCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.InputText))
        {
            // Log a warning for the null or empty input text
            

            // Return an error response indicating the input is invalid
            var errorResponse = new ErrorResponse
            {
                Message = "Input text cannot be null or empty.",
                ErrorCode = "InvalidInput"
            };

            return (null, errorResponse);
        }
        try
        {
            var result = _expenseService.ProcessExpense(request.InputText);
            return await Task.FromResult(result);
        }
        catch (ArgumentException ex)
        {
            // Handle known argument exceptions, such as missing fields or invalid input
            var errorResponse = new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "InvalidArgument"
            };


            return (null, errorResponse);
        }
        catch (XmlException ex)
        {
            // Handle XML parsing exceptions
            var errorResponse = new ErrorResponse
            {
                Message = "Invalid XML format. Please check the input text.",
                ErrorCode = "InvalidXmlFormat"
            };

            return (null, errorResponse);
        }
    }
}
