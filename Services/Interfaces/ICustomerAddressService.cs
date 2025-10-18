using StockManagement.Entities;

namespace StockManagement.Services.Interfaces;

public interface ICustomerAddressService
{
    Task<IEnumerable<CustomerAddress>> GetAddressesByCustomerIdAsync(string customerId);
}