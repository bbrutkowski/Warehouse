using Warehouse.Models;

namespace Warehouse.BussinessLogic.Interfaces
{
    public interface IInventoryService
    {
        Task<IReadOnlyCollection<Inventory>> GetInventoryAsync();
        Task SaveInventoryAsync(IReadOnlyCollection<Inventory> inventories);
    }
}
