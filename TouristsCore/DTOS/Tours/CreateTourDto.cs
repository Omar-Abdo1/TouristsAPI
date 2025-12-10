using System.ComponentModel.DataAnnotations;

namespace TouristsCore.DTOS.Tours;

public class CreateTourDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [Range(0, 100000)]
    public decimal Price { get; set; }

    [Required]
    public int DurationMinutes { get; set; } // e.g., 120 for 2 hours

    [Required]
    public string City { get; set; }

    [Required]
    public string Country { get; set; }
    
    [Required]
    public int MaxGroupSize { get; set; }
        
    // The IDs of photos/videos previously uploaded by user :)
    public List<int> MediaIds { get; set; } = new List<int>();
}