using System.ComponentModel.DataAnnotations;

namespace TouristsCore.Entities;

public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BookingId { get; set; }
    public Booking Booking { get; set; }

    public Guid GuideId { get; set; } 
    public GuideProfile Guide { get; set; } 
    
    [Range(1,5)]
    public int Rating { get; set; }
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
}