using Microsoft.EntityFrameworkCore;
using StockManagement.Context;
using StockManagement.Entities;
using StockManagement.Services.Interfaces;

namespace StockManagement.Services;

public class CustomerAddressService : ICustomerAddressService
{
    private readonly ApplicationDbContext _context;

    public CustomerAddressService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerAddress>> GetAddressesByCustomerIdAsync(string customerId)
    {
        return await _context.CustomerAddresses
            .Where(a => a.CustomerId == customerId && a.IsActive)
            .ToListAsync();
    }
}