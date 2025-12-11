using System.ComponentModel.DataAnnotations;

namespace TouristsCore.DTOS.Schedule;

public class CreateScheduleDto
{
    [Required]
    public  DateTime StartTime { get; set; }
    [Range(1,100)]
    public int Capacity { get; set; }
}