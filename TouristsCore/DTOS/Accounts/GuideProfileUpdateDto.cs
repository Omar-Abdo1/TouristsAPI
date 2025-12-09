namespace TouristsCore.DTOS.Accounts;

public class GuideProfileUpdateDto
{
    public string Bio { get; set; }
    public string?FullName { get; set; }
    public int? ExperienceYears { get; set; }
    public decimal? RatePerHour { get; set; }
    public List<string> Languages { get; set; } 
}