using TouristsCore.DTOS.Schedule;

namespace TouristsCore.DTOS.Tours;

public class TourDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public string City { get; set; }
    public string GuideName { get; set; }
    public string GuideAvatarUrl { get; set; }
    
    public int MaxGroupSize { get; set; }
    
    public bool IsPublished { get; set; }
    public List<string> ImageUrls { get; set; } 
    public List<ScheduleOptionDto> AvailableSchedules { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}