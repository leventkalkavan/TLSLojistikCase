namespace StockManagement.Models.ViewModels.OrderDetailModels;

public class CustomerOrderInfo
{
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public List<string> Addresses { get; set; } = new();
    public int OrderCount { get; set; }
}