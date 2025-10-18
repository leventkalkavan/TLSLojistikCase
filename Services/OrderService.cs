using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StockManagement.Context;
using StockManagement.Entities;
using StockManagement.Hubs;
using StockManagement.Models.ViewModels.OrderModels;
using StockManagement.Services.Interfaces;

namespace StockManagement.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Customer> _userManager;
    private readonly ICustomerAddressService _addressService;
    private readonly IStockService _stockService;
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderService(
        ApplicationDbContext context,
        UserManager<Customer> userManager,
        ICustomerAddressService addressService,
        IStockService stockService,
        IHubContext<OrderHub> hubContext)
    {
        _context = context;
        _userManager = userManager;
        _addressService = addressService;
        _stockService = stockService;
        _hubContext = hubContext;
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
        var activeOrders = await _context.Orders
            .Include(o => o.Customer)
            .Where(o => o.IsActive)
            .GroupBy(o => o.CustomerId)
            .Select(g => new CustomerOrderSummaryViewModel
            {
                CustomerId = g.Key,
                CustomerName = g.First().Customer.Name,
                CustomerEmail = g.First().Customer.Email,
                OrderCount = g.Count(),
                CancelledOrderCount = 0,
                TotalAmount = g.Sum(o => o.TotalPrice - o.Tax),
                TotalTax = g.Sum(o => o.Tax),
                GrandTotal = g.Sum(o => o.TotalPrice),
                LastOrderDate = g.Max(o => o.OrderDate)
            })
            .ToListAsync();

        var cancelledOrders = await _context.Orders
            .Include(o => o.Customer)
            .Where(o => !o.IsActive)
            .GroupBy(o => o.CustomerId)
            .Select(g => new { CustomerId = g.Key, Count = g.Count() })
            .ToListAsync();

        foreach (var activeOrder in activeOrders)
        {
            var cancelledCount = cancelledOrders.FirstOrDefault(c => c.CustomerId == activeOrder.CustomerId)?.Count ?? 0;
            activeOrder.CancelledOrderCount = cancelledCount;
        }

        var customersWithOnlyCancelledOrders = cancelledOrders
            .Where(c => !activeOrders.Any(a => a.CustomerId == c.CustomerId))
            .Select(async c =>
            {
                var customer = await _context.Customers.FindAsync(c.CustomerId);
                return new CustomerOrderSummaryViewModel
                {
                    CustomerId = c.CustomerId,
                    CustomerName = customer.Name,
                    CustomerEmail = customer.Email,
                    OrderCount = 0,
                    CancelledOrderCount = c.Count,
                    TotalAmount = 0,
                    TotalTax = 0,
                    GrandTotal = 0,
                    LastOrderDate = DateTime.MinValue
                };
            });

        var result = activeOrders.ToList();
        foreach (var cancelled in await Task.WhenAll(customersWithOnlyCancelledOrders))
        {
            result.Add(cancelled);
        }

        return result.OrderByDescending(c => c.GrandTotal);
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

        await _hubContext.Clients.All.SendAsync("NewOrderCreated", new
        {
            OrderNo = order.OrderNo,
            CustomerName = customer.Name,
            TotalPrice = order.TotalPrice,
            OrderDate = order.OrderDate
        });

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

    public async Task<bool> CancelOrderAsync(Guid orderId, string customerId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId && o.IsActive);

        if (order == null)
            return false;

        order.IsActive = false;
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("OrderCancelled", new
        {
            OrderNo = order.OrderNo,
            CustomerName = order.Customer.Name,
            OrderId = order.Id
        });

        return true;
    }
}
