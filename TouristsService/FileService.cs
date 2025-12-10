using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TouristsCore;
using TouristsCore.Entities;
using TouristsCore.Services;
using TouristsRepository;

namespace TouristsService;

public class FileService : IFileService
{
    private readonly TouristsContext _touristsContext;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IUnitOfWork _unitOfWork;

    private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".mp4", ".mov" };
    
    public FileService(TouristsContext  touristsContext,IWebHostEnvironment  hostingEnvironment,IUnitOfWork  unitOfWork)
    {
        _touristsContext = touristsContext;
        _hostingEnvironment = hostingEnvironment;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<FileRecord> SaveFileAsync(IFormFile file, string folderName,Guid userId)
    {
        
        if(file==null || file.Length==0)
            throw new ArgumentException("file is null or empty");
        
        folderName = folderName.Trim().ToLower();
        
        var extension = Path.GetExtension(file.FileName).Trim().ToLower();

        if(!allowedExtensions.Contains(extension))
        {
            string allowedlist = string.Join(",", allowedExtensions);
            throw new ArgumentException($"File type '{extension}' is not allowed. Allowed extensions are: {allowedlist} ");
        }     
        
        string uploadFoler = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", folderName); // from home in linux or /C on windows ... ~/wwwroot/upload/foldername

        if (!Directory.Exists(uploadFoler))
            Directory.CreateDirectory(uploadFoler);

        string uniqueFileName = $"{Guid.NewGuid().ToString()}{extension}" ;
        string filePath = Path.Combine(uploadFoler,uniqueFileName);

        using (var filestream = new FileStream(filePath, FileMode.Create)) // save it to disk
        {
            await file.CopyToAsync(filestream);
        }

        var fileRecord = new FileRecord
        {
            UserId = userId,
            FileName = uniqueFileName,
            OriginalName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Length,
            FilePath = $"/uploads/{folderName}/{uniqueFileName}",
            CreatedAt = DateTime.UtcNow
        };
        _touristsContext.FileRecords.Add(fileRecord);
        await _touristsContext.SaveChangesAsync();
        return fileRecord;
    }

    public async Task<bool> DeleteFileAsync(int id,Guid userId,bool isAdmin)
    {

        var fileRecord = await _unitOfWork.Repository<FileRecord>().GetByIdAsync(id);
        
        if (fileRecord == null)
            throw new KeyNotFoundException($"file with id {id} not found");

        if (fileRecord.UserId != userId && !isAdmin)
            throw new UnauthorizedAccessException("You do not own this file.");

        _unitOfWork.Repository<FileRecord>().Delete(fileRecord);
        
        await _unitOfWork.CompleteAsync();
        
        // TODO: Implement this in Phase 4 (Background Jobs) 
        // todo here must invoke the background job that delete physically from server 
        
        return true;
    }
}