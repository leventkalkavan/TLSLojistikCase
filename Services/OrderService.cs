using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockManagement.Context;
using StockManagement.Entities;
using StockManagement.Models.ViewModels.OrderModels;
using StockManagement.Services.Interfaces;

namespace StockManagement.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Customer> _userManager;
    private readonly ICustomerAddressService _addressService;
    private readonly IStockService _stockService;

    public OrderService(
        ApplicationDbContext context,
        UserManager<Customer> userManager,
        ICustomerAddressService addressService,
        IStockService stockService)
    {
        _context = context;
        _userManager = userManager;
        _addressService = addressService;
        _stockService = stockService;
    }

    public async Task<IEnumerable<OrderViewModel>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderNo = o.OrderNo,
                CustomerName = o.Customer.Name,
                TotalPrice = o.TotalPrice,
                Tax = o.Tax,
                OrderDate = o.OrderDate
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<CustomerOrderSummaryViewModel>> GetCustomerOrderSummaryAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Where(o => o.IsActive)
            .GroupBy(o => o.CustomerId)
            .Select(g => new CustomerOrderSummaryViewModel
            {
                CustomerId = g.Key,
                CustomerName = g.First().Customer.Name,
                CustomerEmail = g.First().Customer.Email,
                OrderCount = g.Count(),
                TotalAmount = g.Sum(o => o.TotalPrice - o.Tax),
                TotalTax = g.Sum(o => o.Tax),
                GrandTotal = g.Sum(o => o.TotalPrice),
                LastOrderDate = g.Max(o => o.OrderDate)
            })
            .OrderByDescending(c => c.GrandTotal)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderViewModel>> GetOrdersByCustomerIdAsync(string customerId)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Where(o => o.CustomerId == customerId && o.IsActive)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderNo = o.OrderNo,
                CustomerName = o.Customer.Name,
                TotalPrice = o.TotalPrice,
                Tax = o.Tax,
                OrderDate = o.OrderDate
            })
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<OrderViewModel> CreateAsync(OrderCreateViewModel model)
    {
        var customer = await _userManager.FindByIdAsync(model.CustomerId);
        if (customer == null)
            throw new Exception("Kullanıcı bulunamadı.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = model.CustomerId,
            DeliveryAddressId = model.DeliveryAddressId,
            InvoiceAddressId = model.InvoiceAddressId,
            OrderDate = DateTime.UtcNow,
            OrderNo = Guid.NewGuid().ToString()[..8],
            IsActive = true
        };

        var stockList = await _stockService.GetStocksByIdsAsync(model.OrderDetails.Select(x => x.StockId).ToList());

        foreach (var detail in model.OrderDetails)
        {
            order.OrderDetails.Add(new OrderDetail
            {
                Id = Guid.NewGuid(),
                StockId = detail.StockId,
                Amount = detail.Amount
            });
        }

        order.TotalPrice = order.OrderDetails.Sum(d => 
            d.Amount * (stockList.First(s => s.Id == d.StockId).Price));
        order.Tax = order.TotalPrice * 0.20m;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return new OrderViewModel
        {
            Id = order.Id,
            OrderNo = order.OrderNo,
            CustomerName = customer.Name,
            TotalPrice = order.TotalPrice,
            Tax = order.Tax,
            OrderDate = order.OrderDate
        };
    }
}
