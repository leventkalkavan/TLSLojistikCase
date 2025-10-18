namespace StockManagement.Models.ViewModels.OrderDetailModels;

public class ProductCustomersViewModel
{
    public Guid StockId { get; set; }
    public string StockName { get; set; }
    public int OrderCount { get; set; }
    public List<CustomerOrderInfo> Customers { get; set; } = new();
}