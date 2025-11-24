namespace TouristsCore.Entities;

public class Tour
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GuideProfileId { get; set; }
    public GuideProfile GuideProfile { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string City { get; set; }    // "Giza"
    public string Country { get; set; } // "Egypt"
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsPublished { get; set; } = false;
    public bool IsDeleted { get; set; } = false; // soft-delete for entire tour (admin/guide)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    // Navigation
    public ICollection<Booking> Bookings { get; set; }
}