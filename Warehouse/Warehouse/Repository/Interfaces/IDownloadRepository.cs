using Warehouse.Models;

namespace Warehouse.Repository.Interfaces
{
    public interface IDownloadRepository
    {
        Task<byte[]> DownloadFileAsync(string url);
        Task SaveFileOnDevice(byte[] content, string filePath);
    }
}
