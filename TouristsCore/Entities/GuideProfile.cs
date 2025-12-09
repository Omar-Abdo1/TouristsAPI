namespace TouristsCore.Entities;

public class GuideProfile : BaseEntity,IHasAvatar
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Bio { get; set; }
    
    public string? FullName { get; set; }
    public int? ExperienceYears { get; set; }
    public decimal? RatePerHour { get; set; }
    
    public string? Phone { get; set; }
    public bool IsVerified { get; set; } = false;
    public int? AvatarFileId { get; set; }
    public FileRecord? AvatarFile { get; set; }
    // Navigation
    public ICollection<Tour> Tours { get; set; }
    public ICollection<GuideLanguage> GuideLanguages { get; set; }
    
}