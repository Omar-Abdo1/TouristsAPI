namespace TouristsCore.DTOS.Booking;

public class BookingTouristDto
{
    public int Id { get; set; }
    public int TicketCount { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
    public DateTime BookingDate { get; set; }
    
    public string TourName { get; set; }
    public string City { get; set; }
    public DateTime TourDate { get; set; }
}