namespace TouristsCore.Entities;

public class Tour : BaseEntity
{
    public int GuideProfileId { get; set; }
    public GuideProfile GuideProfile { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string City { get; set; }    // "Giza"
    public string Country { get; set; } // "Egypt"
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsPublished { get; set; } = false;
    
    public int MaxGroupSize { get; set; }
    
    
    // Navigation
    public ICollection<Booking> Bookings { get; set; }
    public ICollection<TourMedia>  Media { get; set; }
    
    public ICollection<TourSchedule> Schedules { get; set; } = new List<TourSchedule>();
    public ICollection<Review>  Reviews { get; set; } = new List<Review>();
}