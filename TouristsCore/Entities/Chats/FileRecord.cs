namespace TouristsCore.Entities;

public class FileRecord : BaseEntity 
{
    public Guid? UserId { get; set; }
    public User?  User { get; set; }
    public string FileName { get; set; }
    public string StoragePath { get; set; } // relative/absolute storage path or cloud URL
    public string ContentType { get; set; }
    public long SizeBytes { get; set; }
    // todo make a background job to delete the files from DB after a week from being deleted
}