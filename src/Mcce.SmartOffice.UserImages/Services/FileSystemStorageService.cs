using Mcce.SmartOffice.Core.Exceptions;
using Serilog;

namespace Mcce.SmartOffice.UserImages.Services
{
    public class FileSystemStorageService : IStorageService
    {
        private readonly string _basePath;

        public FileSystemStorageService(string basePath)
        {
            _basePath = basePath;

            if (Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }

        public async Task<string[]> GetFiles(string path)
        {
            var directoryPath = Path.Combine(_basePath, path);

            return await Task.FromResult(Directory.Exists(directoryPath) ? Directory.GetFiles(directoryPath) : Array.Empty<string>());
        }

        public async Task<bool> FileExists(string path)
        {
            var filePath = Path.Combine(_basePath, path);

            Log.Debug($"Checking if file '{filePath}' exists...");

            return await Task.FromResult(File.Exists(filePath));
        }

        public async Task<Stream> GetContent(string path)
        {
            var filePath = Path.Combine(_basePath, path);

            if (!File.Exists(filePath))
            {
                throw new NotFoundException($"The file '{path}' could not be found!");
            }

            return await Task.FromResult(new FileStream(filePath, FileMode.Open, FileAccess.Read));
        }

        public async Task StoreContent(string path, Stream stream)
        {
            var directoryPath = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);

            var fullDirectoryPath = Path.Combine(_basePath, directoryPath);
            var fullFilePath = Path.Combine(fullDirectoryPath, fileName);

            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            using var fs = new FileStream(fullFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            await stream.CopyToAsync(fs);
        }

        public async Task DeleteContent(string path)
        {
            var filePath = Path.Combine(_basePath, path);

            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }            
        }

        public async Task DeleteDirectory(string path)
        {
            var directoryPath = Path.Combine(_basePath, path);

            if (Directory.Exists(directoryPath))
            {
                await Task.Run(() => Directory.Delete(directoryPath, true));
            }            
        }
    }
}
