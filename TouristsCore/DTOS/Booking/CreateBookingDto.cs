using System.ComponentModel.DataAnnotations;

namespace TouristsCore.DTOS.Booking;

public class CreateBookingDto
{
    [Required]
    public int ScheduleId { get; set; }

    [Required]
    [Range(1, 20, ErrorMessage = "You can book between 1 and 20 tickets.")]
    public int TicketCount { get; set; }
}