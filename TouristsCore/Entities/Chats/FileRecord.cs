namespace TouristsCore.Entities;

public class FileRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public User?  User { get; set; }
    public string FileName { get; set; }
    public string StoragePath { get; set; } // relative/absolute storage path or cloud URL
    public string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false; 
    // todo make a background job to delete the files from DB after a week from being deleted
}