using System.ComponentModel.DataAnnotations;

namespace TouristsCore.Entities;

public class Review : BaseEntity
{

    public int BookingId { get; set; }
    public Booking Booking { get; set; }

    public int GuideId { get; set; } 
    public GuideProfile Guide { get; set; } 
    
    [Range(1,5)]
    public int Rating { get; set; }
    public string? Comment { get; set; }
}