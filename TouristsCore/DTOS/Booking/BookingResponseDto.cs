namespace TouristsCore.DTOS.Booking;

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public string TourName { get; set; }
    public DateTime TourDate { get; set; }
    public int Tickets { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
}