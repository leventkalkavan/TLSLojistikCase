using Microsoft.EntityFrameworkCore;
using StockManagement.Context;
using StockManagement.Models.ViewModels.OrderDetailModels;
using StockManagement.Services.Interfaces;

namespace StockManagement.Services;

public class OrderDetailService : IOrderDetailService
{
    private readonly ApplicationDbContext _context;

    public OrderDetailService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductCustomersViewModel>> GetProductCustomersAsync()
    {
        var orderDetails = await _context.OrderDetails
            .Include(od => od.Stock)
            .Include(od => od.Order)
            .ThenInclude(o => o.Customer)
            .Include(od => od.Order)
            .ThenInclude(o => o.DeliveryAddress)
            .Where(od => od.IsActive && od.Order.IsActive)
            .ToListAsync();

        var productCustomers = orderDetails
            .GroupBy(od => od.StockId)
            .Select(g => new ProductCustomersViewModel
            {
                StockId = g.Key,
                StockName = g.First().Stock.Name,
                OrderCount = g.Count(),
                Customers = g.GroupBy(od => od.Order.CustomerId)
                    .Select(customerGroup => new CustomerOrderInfo
                    {
                        CustomerName = customerGroup.First().Order.Customer.Name,
                        CustomerEmail = customerGroup.First().Order.Customer.Email,
                        Addresses = customerGroup.Select(od => 
                            $"{od.Order.DeliveryAddress.Address}, {od.Order.DeliveryAddress.Town}, {od.Order.DeliveryAddress.City}")
                            .Distinct()
                            .ToList(),
                        OrderCount = customerGroup.Count()
                    })
                    .ToList()
            })
            .Where(pc => pc.OrderCount > 1)
            .OrderByDescending(pc => pc.OrderCount)
            .ToList();

        return productCustomers;
    }

    public async Task<IEnumerable<MultiQuantityOrdersViewModel>> GetMultiQuantityOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Stock)
            .Where(o => o.IsActive && o.OrderDetails.Any(od => od.Amount > 1 && od.IsActive))
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var multiQuantityOrders = orders.Select(order => new MultiQuantityOrdersViewModel
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo,
            CustomerName = order.Customer.Name,
            CustomerEmail = order.Customer.Email,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            Tax = order.Tax,
            DeliveryAddress = $"{order.DeliveryAddress.Address}, {order.DeliveryAddress.Town}, {order.DeliveryAddress.City}",
            OrderDetails = order.OrderDetails
                .Where(od => od.Amount > 1 && od.IsActive)
                .Select(od => new OrderDetailInfo
                {
                    StockName = od.Stock.Name,
                    Amount = od.Amount,
                    UnitPrice = od.Stock.Price,
                    TotalPrice = od.Amount * od.Stock.Price
                })
                .ToList()
        }).ToList();

        return multiQuantityOrders;
    }

    public async Task<IEnumerable<DifferentAddressOrdersViewModel>> GetDifferentAddressOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.InvoiceAddress)
            .Where(o => o.IsActive && o.DeliveryAddressId != o.InvoiceAddressId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var differentAddressOrders = orders.Select(order => new DifferentAddressOrdersViewModel
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo,
            CustomerName = order.Customer.Name,
            CustomerEmail = order.Customer.Email,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            DeliveryAddress = $"{order.DeliveryAddress.Address}, {order.DeliveryAddress.Town}, {order.DeliveryAddress.City}",
            InvoiceAddress = $"{order.InvoiceAddress.Address}, {order.InvoiceAddress.Town}, {order.InvoiceAddress.City}"
        }).ToList();

        return differentAddressOrders;
    }

    public async Task<IEnumerable<TLSOrdersViewModel>> GetTLSOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.InvoiceAddress)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Stock)
            .Where(o => o.IsActive && o.Customer.Name == "TLS Ornek")
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var tlsOrders = orders.Select(order => new TLSOrdersViewModel
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo,
            CustomerName = order.Customer.Name,
            CustomerEmail = order.Customer.Email,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            Tax = order.Tax,
            DeliveryAddress = $"{order.DeliveryAddress.Address}, {order.DeliveryAddress.Town}, {order.DeliveryAddress.City}",
            InvoiceAddress = $"{order.InvoiceAddress.Address}, {order.InvoiceAddress.Town}, {order.InvoiceAddress.City}",
            OrderDetails = order.OrderDetails
                .Where(od => od.IsActive)
                .Select(od => new OrderDetailInfo
                {
                    StockName = od.Stock.Name,
                    Amount = od.Amount,
                    UnitPrice = od.Stock.Price,
                    TotalPrice = od.Amount * od.Stock.Price
                })
                .ToList()
        }).ToList();

        return tlsOrders;
    }

    public async Task<IEnumerable<IstanbulOrdersViewModel>> GetIstanbulOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.InvoiceAddress)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Stock)
            .Where(o => o.IsActive && o.DeliveryAddress.City == "İstanbul")
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var istanbulOrders = orders.Select(order => new IstanbulOrdersViewModel
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo,
            CustomerName = order.Customer.Name,
            CustomerEmail = order.Customer.Email,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            Tax = order.Tax,
            DeliveryAddress = $"{order.DeliveryAddress.Address}, {order.DeliveryAddress.Town}, {order.DeliveryAddress.City}",
            InvoiceAddress = $"{order.InvoiceAddress.Address}, {order.InvoiceAddress.Town}, {order.InvoiceAddress.City}",
            OrderDetails = order.OrderDetails
                .Where(od => od.IsActive)
                .Select(od => new OrderDetailInfo
                {
                    StockName = od.Stock.Name,
                    Amount = od.Amount,
                    UnitPrice = od.Stock.Price,
                    TotalPrice = od.Amount * od.Stock.Price
                })
                .ToList()
        }).ToList();

        return istanbulOrders;
    }

    public async Task<IEnumerable<CustomerOrderDetailsViewModel>> GetCustomerOrderDetailsAsync(string customerId)
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.InvoiceAddress)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Stock)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var customerOrderDetails = orders.Select(order => new CustomerOrderDetailsViewModel
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo,
            CustomerName = order.Customer.Name,
            CustomerEmail = order.Customer.Email,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            Tax = order.Tax,
            DeliveryAddress = $"{order.DeliveryAddress.Address}, {order.DeliveryAddress.Town}, {order.DeliveryAddress.City}",
            InvoiceAddress = $"{order.InvoiceAddress.Address}, {order.InvoiceAddress.Town}, {order.InvoiceAddress.City}",
            IsActive = order.IsActive,
            OrderDetails = order.OrderDetails
                .Where(od => od.IsActive)
                .Select(od => new OrderDetailInfo
                {
                    StockName = od.Stock.Name,
                    Amount = od.Amount,
                    UnitPrice = od.Stock.Price,
                    TotalPrice = od.Amount * od.Stock.Price
                })
                .ToList()
        }).ToList();

        return customerOrderDetails;
    }
}
