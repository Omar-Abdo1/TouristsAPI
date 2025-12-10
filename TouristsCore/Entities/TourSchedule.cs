namespace TouristsCore.Entities;

public class TourSchedule : BaseEntity
{
    public int TourId { get; set; }
    public Tour Tour { get; set; }

    public DateTime StartTime { get; set; }
    
    public int AvailableSeats { get; set; }
    
    public ICollection<Booking> Bookings { get; set; }
}