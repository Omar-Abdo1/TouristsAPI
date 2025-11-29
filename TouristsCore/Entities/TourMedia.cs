namespace TouristsCore.Entities;

public class TourMedia : BaseEntity
{

    public int TourId { get; set; }
    public Tour Tour { get; set; }

    public int FileId { get; set; }
    public FileRecord File { get; set; }

    public bool IsVideo { get; set; } = false; 
    
    public int OrderIndex { get; set; }
}