using Microsoft.AspNetCore.Http;
using TouristsCore.Entities;

namespace TouristsCore.Services;

public interface IFileService
{
    Task<FileRecord> SaveFileAsync(IFormFile file, string folderName,Guid userId);
    Task DeleteFileAsync(string fileName,string folderName);
}