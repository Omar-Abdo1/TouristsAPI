namespace TouristsCore.Entities;

public class GuideProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Bio { get; set; }
    public int? ExperienceYears { get; set; }
    public decimal? RatePerHour { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? AvatarFileId { get; set; }
    public FileRecord? AvatarFile { get; set; }
    // Navigation
    public ICollection<Tour> Tours { get; set; }
    public ICollection<Language>  Languages { get; set; }
}