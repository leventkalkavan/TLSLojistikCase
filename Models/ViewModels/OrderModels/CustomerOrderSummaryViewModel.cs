namespace StockManagement.Models.ViewModels.OrderModels;

public class CustomerOrderSummaryViewModel
{
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public int OrderCount { get; set; }
    public int CancelledOrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime LastOrderDate { get; set; }
}
