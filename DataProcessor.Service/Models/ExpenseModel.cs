namespace DataProcessor.Service.Models;

public class ExpenseModel
{
    public string CostCentre { get; set; } = "UNKNOWN";
    public decimal Total { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal SalesTax { get; set; }
    public decimal TotalExcludingTax { get; set; }
    public string? Vendor { get; set; }
    public string? Description { get; set; }
    public DateTime? Date { get; set; }
    public Dictionary<string, string> UnknownTags { get; set; } = new Dictionary<string, string>();
}
