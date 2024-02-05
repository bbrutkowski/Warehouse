using Warehouse.BussinessLogic.Interfaces;
using Warehouse.Repository.Interfaces;

namespace Warehouse.BussinessLogic
{
    public class DownloadService : IDownloadService
    {
        private readonly IDownloadRepository _downloadRepository;

        public DownloadService(IDownloadRepository downloadRepository) 
        {
            _downloadRepository = downloadRepository;
        }

        public async Task<byte[]> DownloadFileAsync(string url)
        {
            return await _downloadRepository.DownloadFileAsync(url);        
        }      
    }
}
