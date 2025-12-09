namespace TouristsCore.DTOS.Tours;

public class TourRequestDto
{
   
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;

    public string? City { get; set; }     
    public decimal? MinPrice { get; set; } 
    public decimal? MaxPrice { get; set; }
    public int? GuideId { get; set; }
        
    // Sorting (e.g., "priceAsc", "priceDesc", "newest")
    public string? SortBy { get; set; }
}