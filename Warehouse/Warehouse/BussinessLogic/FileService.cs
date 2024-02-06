using CSharpFunctionalExtensions;
using Warehouse.BussinessLogic.Interfaces;

namespace Warehouse.BussinessLogic
{
    public class FileService : IFileService
    {
        private readonly IHttpClientFactory _clientFactory;

        public FileService(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<Result<byte[]>> DownloadFileAsync(string url, CancellationToken token)
        {
            var httpClient = _clientFactory.CreateClient();
         
            try
            {
                byte[] fileBytes = await httpClient.GetByteArrayAsync(url, token);
                return Result.Success(fileBytes);
            }
            catch (Exception e)
            {
                return Result.Failure<byte[]>(e.Message);            
            }
        }

        public async Task<Result> SaveFileOnDeviceAsync(byte[] content, string filePath, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filePath)) return Result.Failure("File path is empty");

            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
                await fileStream.WriteAsync(content, token);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<string>> GetSavedFileAsync(string filePath, CancellationToken token)
        {
            try
            {
                string rawCsvData = await File.ReadAllTextAsync(filePath, token);
                return Result.Success(rawCsvData);
            }
            catch (Exception ex)
            {
                return Result.Failure<string>($"An error occurred while reading the file: {ex.Message}");
            }
        }
    }
}
