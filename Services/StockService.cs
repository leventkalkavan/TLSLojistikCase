using Microsoft.EntityFrameworkCore;
using StockManagement.Context;
using StockManagement.Entities;
using StockManagement.Services.Interfaces;

namespace StockManagement.Services;

public class StockService : IStockService
{
    private readonly ApplicationDbContext _context;

    public StockService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Stock>> GetAllStocksAsync()
    {
        return await _context.Stocks.Where(s => s.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Stock>> GetStocksByIdsAsync(IEnumerable<Guid> stockIds)
    {
        return await _context.Stocks
            .Where(s => stockIds.Contains(s.Id) && s.IsActive)
            .ToListAsync();
    }
}