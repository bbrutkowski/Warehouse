using Warehouse.Models;

namespace Warehouse.Repository.Interfaces
{
    public interface IInventoryRepository
    {
        Task<bool> SaveInventory(IReadOnlyCollection<Inventory> inventories);
        Task<List<Inventory>> GetInventoryAsync(string url);
    }
}
