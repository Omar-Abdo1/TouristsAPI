using Microsoft.AspNetCore.Http;
using TouristsCore.Entities;

namespace TouristsCore.Services;

public interface IFileService
{
    Task<FileRecord> SaveFileAsync(IFormFile file, string folderName,Guid userId);
    Task<bool> DeleteFileAsync(int id,Guid userId,bool isAdmin);
}