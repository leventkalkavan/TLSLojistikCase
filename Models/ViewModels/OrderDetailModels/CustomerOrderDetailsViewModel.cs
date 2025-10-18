namespace StockManagement.Models.ViewModels.OrderDetailModels;

public class CustomerOrderDetailsViewModel
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal Tax { get; set; }
    public string DeliveryAddress { get; set; }
    public string InvoiceAddress { get; set; }
    public List<OrderDetailInfo> OrderDetails { get; set; } = new();
}
