namespace TouristsCore.DTOS.Schedule;

public class ScheduleResponseDto
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public int AvailableSeats { get; set; }
    public int TourId { get; set; }
}