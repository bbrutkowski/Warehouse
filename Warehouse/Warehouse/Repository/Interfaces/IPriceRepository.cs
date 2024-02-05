using Warehouse.Models;

namespace Warehouse.Repository.Interfaces
{
    public interface IPriceRepository
    {
        Task<IReadOnlyCollection<Price>> GetPricesAsync(string url);
        Task<bool> SavePricesAsync(IReadOnlyCollection<Price> prices);
    }
}
