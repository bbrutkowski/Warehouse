using Warehouse.Models;

namespace Warehouse.BussinessLogic.Interfaces
{
    public interface IPriceService
    {
        Task<IReadOnlyCollection<Price>> GetPricesAsync();
        Task<bool> SavePricesAsync(IReadOnlyCollection<Price> prices);
    }
}
