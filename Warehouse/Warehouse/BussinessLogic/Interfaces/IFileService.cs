using CSharpFunctionalExtensions;

namespace Warehouse.BussinessLogic.Interfaces
{
    public interface IFileService
    {
        Task<Result<byte[]>> DownloadFileAsync(string url, CancellationToken token);
        Task<Result> SaveFileOnDeviceAsync(byte[] content, string filePath, CancellationToken token);
        Task<Result<string>> GetSavedFileAsync(string filePath, CancellationToken token);
    }
}
