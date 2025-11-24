using TouristsCore.Enums;

namespace TouristsCore.Entities;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TouristId { get; set; }
    public User Tourist { get; set; }
    public Guid GuideId { get; set; }
    public GuideProfile Guide { get; set; }
    public Guid TourId { get; set; }
    public Tour Tour { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public BookingStatus Status { get; set; } // Pending / Paid / Active / Completed / Cancelled
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Navigation
    public Payment Payment { get; set; }
}