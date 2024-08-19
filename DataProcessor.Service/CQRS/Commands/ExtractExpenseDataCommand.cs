using DataProcessor.Service.Models;
using MediatR;
using System.Xml.Serialization;

namespace DataProcessor.Service.CQRS.Commands;
public class ExtractExpenseDataCommand : IRequest<(ExpenseModel Expense, ErrorResponse Error)>
{
    public string? InputText { get; set; }
}

