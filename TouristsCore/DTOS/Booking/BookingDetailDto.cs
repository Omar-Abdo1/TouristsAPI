namespace TouristsCore.DTOS.Booking;

public class BookingDetailDto
{
    public int Id { get; set; }
    public string Status { get; set; }
    public DateTime BookingDate { get; set; }
    
    public int TicketCount { get; set; }
    public decimal PricePaid { get; set; }
    
    public int TourId { get; set; }
    public string TourName { get; set; }
    public string TourDescription { get; set; }
    public string City { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public string TouristName { get; set; } 
    public string GuideName { get; set; } 
}