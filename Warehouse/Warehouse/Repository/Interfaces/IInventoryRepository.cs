using CSharpFunctionalExtensions;
using Warehouse.Models;

namespace Warehouse.Repository.Interfaces
{
    public interface IInventoryRepository
    {
        Task<Result> SaveInventoryAsync(IReadOnlyCollection<Inventory> inventories);
    }
}
