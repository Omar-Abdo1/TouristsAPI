namespace TouristsCore.Entities;

public class Tour : BaseEntity
{
    public Guid GuideProfileId { get; set; }
    public GuideProfile GuideProfile { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string City { get; set; }    // "Giza"
    public string Country { get; set; } // "Egypt"
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsPublished { get; set; } = false;
    // Navigation
    public ICollection<Booking> Bookings { get; set; }
    public ICollection<TourMedia>  Media { get; set; }
}