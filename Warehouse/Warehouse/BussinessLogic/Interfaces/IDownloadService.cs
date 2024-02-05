namespace Warehouse.BussinessLogic.Interfaces
{
    public interface IDownloadService 
    {
        Task<byte[]> DownloadFileAsync(string url);
    }
}
