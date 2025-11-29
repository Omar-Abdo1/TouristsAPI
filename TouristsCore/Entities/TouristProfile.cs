namespace TouristsCore.Entities;

public class TouristProfile : BaseEntity // optional to Fill 
{

    public Guid UserId { get; set; }
    public User User { get; set; }

    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Country { get; set; }
    public int? AvatarFileId { get; set; }
    public FileRecord? AvatarFile { get; set; }
    
    ICollection<Booking>?  Bookings { get; set; }
}