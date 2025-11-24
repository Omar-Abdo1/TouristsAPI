namespace TouristsCore.Entities;

public class TouristProfile // optional to Fill 
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; }

    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Country { get; set; }
    public Guid? AvatarFileId { get; set; }
    public FileRecord? AvatarFile { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}