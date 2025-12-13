using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TouristsCore.Entities;

[Index(nameof(BookingId), IsUnique = true)] // 1 review per 1 booking
public class Review : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; }
    
    public int TourId { get; set; } 
    public Tour Tour { get; set; }

    public int GuideId { get; set; } 
    public GuideProfile Guide { get; set; } 
    
    public int TouristId { get; set; }
    public TouristProfile Tourist { get; set; }
    
    [Range(1,5)]
    public int Rating { get; set; }
    public string? Comment { get; set; }
}