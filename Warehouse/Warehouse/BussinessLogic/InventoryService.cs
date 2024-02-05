using Warehouse.BussinessLogic.Interfaces;
using Warehouse.Models;
using Warehouse.Repository.Interfaces;

namespace Warehouse.BussinessLogic
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        private const string InventoryFileUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv";

        public InventoryService(IInventoryRepository inventoryRepository) => _inventoryRepository = inventoryRepository;

        public async Task<IReadOnlyCollection<Inventory>> GetInventoryAsync()
        {
            var inventoryList = await _inventoryRepository.GetInventoryAsync(InventoryFileUrl);

            return inventoryList.Where(x => x.Shipping == "24h").ToList();
        }

        public async Task SaveInventoryAsync(IReadOnlyCollection<Inventory> inventories)
        {
            await _inventoryRepository.SaveInventory(inventories);
        }
    }
}
