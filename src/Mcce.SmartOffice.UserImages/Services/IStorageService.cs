namespace Mcce.SmartOffice.UserImages.Services
{
    public interface IStorageService
    {
        Task<string[]> GetFiles(string path);

        Task<bool> FileExists(string path);

        Task<Stream> GetContent(string path);

        Task StoreContent(string path, Stream stream);

        Task DeleteContent(string path);

        Task DeleteDirectory(string path);
    }
}
