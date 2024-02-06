using CSharpFunctionalExtensions;
using Warehouse.Models;

namespace Warehouse.Repository.Interfaces
{
    public interface IPriceRepository
    {
        Task<Result> SavePricesAsync(IReadOnlyCollection<Price> prices);
    }
}
