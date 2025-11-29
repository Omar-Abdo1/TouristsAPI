using TouristsCore.Enums;

namespace TouristsCore.Entities;

public class Booking : BaseEntity
{
    public int TouristId { get; set; }
    public User Tourist { get; set; }
    public Guid GuideId { get; set; }
    public GuideProfile Guide { get; set; }
    public int TourId { get; set; }
    public Tour Tour { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public BookingStatus Status { get; set; } // Pending / Paid / Active / Completed / Cancelled
    public decimal TotalPrice { get; set; }
    // Navigation
    public Payment Payment { get; set; }
}