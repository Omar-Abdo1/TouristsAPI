namespace TouristsCore.DTOS.Booking;

public class GuideSalesDto
{
    public int BookingId { get; set; }
    public string TouristName { get; set; } 
    public DateTime BookingDate { get; set; }
    
    public DateTime TourDate { get; set; }
    
    public int TicketCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public string Status { get; set; }
}