namespace StockManagement.Models.ViewModels.OrderModels;

public class OrderViewModel
{
    public Guid Id { get; set; }
    public string OrderNo { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal Tax { get; set; }
    public DateTime OrderDate { get; set; }
}