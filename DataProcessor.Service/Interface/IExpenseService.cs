using DataProcessor.Service.Models;

namespace DataProcessor.Service.Interface;

public interface IExpenseService
{
    (ExpenseModel expense, ErrorResponse error) ProcessExpense(string inputText);
}
