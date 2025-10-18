using StockManagement.Entities;
using StockManagement.Models.ViewModels.OrderDetailModels;

namespace StockManagement.Services.Interfaces;

public interface IOrderDetailService
{
    Task<IEnumerable<ProductCustomersViewModel>> GetProductCustomersAsync();
    Task<IEnumerable<MultiQuantityOrdersViewModel>> GetMultiQuantityOrdersAsync();
    Task<IEnumerable<DifferentAddressOrdersViewModel>> GetDifferentAddressOrdersAsync();
    Task<IEnumerable<TLSOrdersViewModel>> GetTLSOrdersAsync();
    Task<IEnumerable<IstanbulOrdersViewModel>> GetIstanbulOrdersAsync();
    Task<IEnumerable<CustomerOrderDetailsViewModel>> GetCustomerOrderDetailsAsync(string customerId);
}