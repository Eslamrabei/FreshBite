using Microsoft.AspNetCore.Http;

namespace ServiceAbstraction.Contracts
{
    public interface IFileService
    {
        Task<string?> UploadFileAsync(IFormFile file, string folderName);
        void DeleteFile(string filePath);
    }
}
