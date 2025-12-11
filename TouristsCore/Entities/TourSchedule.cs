using System.ComponentModel.DataAnnotations;

namespace TouristsCore.Entities;

public class TourSchedule : BaseEntity
{
    public int TourId { get; set; }
    public Tour Tour { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public int AvailableSeats { get; set; }
    
    [Timestamp]
    public byte[] RowVersion { get; set; } // for concurrency when booking
    
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}