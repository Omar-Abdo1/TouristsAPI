using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    [Column(TypeName = "decimal(18,2)")]
    public decimal PriceAtBooking { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending; // Pending / Paid / Confirmed / Cancelled
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    
    public Payment Payment { get; set; }
    
    [Timestamp]
    public byte[] RowVersion { get; set; } // for concurrency when Canceling or Buying
}