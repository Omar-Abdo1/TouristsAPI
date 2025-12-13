using System.ComponentModel.DataAnnotations;

namespace TouristsCore.DTOS.Reviews;

public class UpdateReviewDto
{
    [Required,Range(1,5)]
    public int Rating { get; set; }
    [MaxLength(500)]
    public string? Comment { get; set; }
}