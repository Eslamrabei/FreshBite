using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Service.Implementations
{
    public class FileService(IWebHostEnvironment _env) : IFileService
    {
        public async Task<string?> UploadFileAsync(IFormFile file, string folderName)
        {
            List<string> AllowedExtension = [".png", ",jpg", ".jpeg"];
            const int MAX_SIZE = 2_097_152;

            var Extension = Path.GetExtension(file.FileName);
            if (!AllowedExtension.Contains(Extension))
                return null;
            if (file.Length == 0 || file.Length > MAX_SIZE)
                return null;

            var FolderPath = Path.Combine(_env.WebRootPath, "images", folderName);
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            var FileName = $"{Guid.NewGuid().ToString()}-{Path.GetExtension(file.FileName)}";
            var FilePath = Path.Combine(FolderPath, FileName);

            using (var fileStream = new FileStream(FilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine("/images", folderName, FileName).Replace("\\", "/");
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            if (File.Exists(filePath))
                File.Delete(filePath);

        }

    }
}
