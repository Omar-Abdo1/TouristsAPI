namespace TouristsCore.DTOS.Reviews;

public class ReviewResponseDto
{
    public int Id { get; set; }
    public string TouristName { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime Date { get; set; }
}