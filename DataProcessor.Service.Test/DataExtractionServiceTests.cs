using DataProcessor.Service.Models;
using DataProcessor.Service.Services;
using Microsoft.Extensions.Options;

public class ExpenseServiceTests
{
    private readonly ExpenseService _expenseService;
    private readonly decimal TaxRate;
    public ExpenseServiceTests()
    {
        TaxRate = 0.10m; // Assuming a 10% sales tax rate
        var salesTaxOptions = Options.Create(new SalesTaxOptions
        {
            Rate = TaxRate  // Assuming a 10% sales tax rate
        });

        // Initialize ExpenseService with the required options
        _expenseService = new ExpenseService(salesTaxOptions);
    }

    [Fact]
    public void ExtractExpenseData_ValidInput_ReturnsExpectedExpense()
    {
        // Arrange
        var inputText = "Hi Patricia,\nPlease create an expense claim for the below. Relevant details are marked up as requested…\n<expense><cost_centre>DEV632</cost_centre><total>35,000</total><payment_method>personal card</payment_method></expense>\nFrom: William Steele\nSent: Friday, 16 June 2022 10:32 AM\nTo: Maria Washington\nSubject: test\nHi Maria,\nPlease create a reservation for 10 at the <vendor>Seaside Steakhouse</vendor> for our <description>development team’s project end celebration</description> on <date>27 April 2022</date> at 7.30pm.\nRegards,\nWilliam";

        // Act
        var result = _expenseService.ProcessExpense(inputText);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("DEV632", result.expense.CostCentre);
        Assert.Equal(35000, result.expense.Total);
        // Assuming you have a tax rate defined and calculated correctly in your service
        var expectedSalesTax = result.expense.Total * TaxRate / (1 + TaxRate);
        var expectedTotalExcludingTax = result.expense.Total - expectedSalesTax;
        Assert.Equal(expectedSalesTax, result.expense.SalesTax);
        Assert.Equal(expectedTotalExcludingTax, result.expense.TotalExcludingTax);
    }

    [Fact]
    public void ExtractExpenseData_MissingTotal_ThrowsArgumentException()
    {
        // Arrange
        var inputText = "<expense><cost_centre>DEV632</cost_centre><payment_method>personal card</payment_method></expense>";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _expenseService.ProcessExpense(inputText));
    }

   
}
