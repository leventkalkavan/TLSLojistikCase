using StockManagement.Entities;
using StockManagement.Models.ViewModels.OrderModels;

namespace StockManagement.Services.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderViewModel>> GetAllAsync();
    Task<IEnumerable<CustomerOrderSummaryViewModel>> GetCustomerOrderSummaryAsync();
    Task<IEnumerable<OrderViewModel>> GetOrdersByCustomerIdAsync(string customerId);
    Task<OrderViewModel> CreateAsync(OrderCreateViewModel model);
    Task<bool> CancelOrderAsync(Guid orderId, string customerId);
}