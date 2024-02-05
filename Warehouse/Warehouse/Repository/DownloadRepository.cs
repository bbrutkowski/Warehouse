using Warehouse.Repository.Interfaces;

namespace Warehouse.Repository
{
    public class DownloadRepository : IDownloadRepository
    {
        public async Task<byte[]> DownloadFileAsync(string url)
        {
            var httpClient = CreateHttpClient();

            using var httpClientFactory = CreateHttpClient();
            byte[] fileBytes = await httpClient.GetByteArrayAsync(url);

            return fileBytes;
        }

        public async Task SaveFileOnDevice(byte[] content, string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            await fileStream.WriteAsync(content);
        }

        private static HttpClient CreateHttpClient()
        {
            var serviceProvider = new ServiceCollection()
                .AddHttpClient()
                .BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            return httpClientFactory.CreateClient();
        }
    }
}
