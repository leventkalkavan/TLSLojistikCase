using StockManagement.Entities;

namespace StockManagement.Services.Interfaces;

public interface IStockService
{
    Task<IEnumerable<Stock>> GetAllStocksAsync();
    Task<IEnumerable<Stock>> GetStocksByIdsAsync(IEnumerable<Guid> stockIds);
}