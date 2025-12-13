namespace TouristsCore.DTOS.Admin;

public class AdminStatsDto
{
    public decimal TotalRevenue { get; set; }
    
    public int TotalBookings { get; set; }
    public int TotalTours { get; set; }
    public int TotalGuides { get; set; }
    public int TotalTourists { get; set; }
}