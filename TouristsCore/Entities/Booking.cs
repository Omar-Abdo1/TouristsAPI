using TouristsCore.Enums;

namespace TouristsCore.Entities;

public class Booking : BaseEntity
{
    public int TouristId { get; set; }
    public TouristProfile Tourist { get; set; }

    public int TourId { get; set; }
    public Tour Tour { get; set; }

    public int? TourScheduleId { get; set; } 
    public TourSchedule TourSchedule { get; set; }
    
    public int TicketCount { get; set; } 
    
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }

    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } // Pending / Paid / Confirmed / Cancelled
    public int GuideId { get; set; }
    public GuideProfile Guide { get; set; }

    public Payment Payment { get; set; }
}