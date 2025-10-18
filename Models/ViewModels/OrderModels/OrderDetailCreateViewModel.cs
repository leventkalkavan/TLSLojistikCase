namespace StockManagement.Models.ViewModels.OrderModels;

public class OrderDetailCreateViewModel
{
    public Guid StockId { get; set; }
    public int Amount { get; set; }
}