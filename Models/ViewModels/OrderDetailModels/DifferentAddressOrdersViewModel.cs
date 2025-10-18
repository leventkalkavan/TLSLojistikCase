namespace StockManagement.Models.ViewModels.OrderDetailModels;

public class DifferentAddressOrdersViewModel
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string DeliveryAddress { get; set; }
    public string InvoiceAddress { get; set; }
}
