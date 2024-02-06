using CSharpFunctionalExtensions;

namespace Warehouse.BussinessLogic.Interfaces
{
    public interface IInventoryService
    {
        Task<Result<byte[]>> GetInventoryFileAsync(CancellationToken token);
        Task<Result> SaveInventoryFileOnDeviceAsync(byte[] inventoryFile, CancellationToken token);
        Task<Result> FilterAndSaveInventoryAsync(CancellationToken token);
    }
}
