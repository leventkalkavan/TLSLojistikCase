namespace StockManagement.Models.ViewModels.OrderModels;

public class OrderCreateViewModel
{
    public string CustomerId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public Guid InvoiceAddressId { get; set; }

    public List<OrderDetailCreateViewModel> OrderDetails { get; set; } = new();
}