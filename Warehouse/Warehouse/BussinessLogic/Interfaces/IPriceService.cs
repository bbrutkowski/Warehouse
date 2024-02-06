using CSharpFunctionalExtensions;
using Warehouse.Models;

namespace Warehouse.BussinessLogic.Interfaces
{
    public interface IPriceService
    {
        Task<Result> SavePriceAsync(CancellationToken token);
        Task<Result<byte[]>> GetPriceFileAsync(CancellationToken token);
        Task<Result> SavePriceFileOnDeviceAsync(byte[] byteFile, CancellationToken token);
    }
}
