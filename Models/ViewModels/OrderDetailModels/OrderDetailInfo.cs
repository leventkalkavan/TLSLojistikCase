namespace StockManagement.Models.ViewModels.OrderDetailModels;

public class OrderDetailInfo
{
    public string StockName { get; set; }
    public int Amount { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}