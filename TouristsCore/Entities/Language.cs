namespace TouristsCore.Entities;

public class Language
{
    public int Id { get; set; }
    public string Name { get; set; }
    public GuideProfile GuideProfile { get; set; }
    public ICollection<GuideProfile> GuideProfiles { get; set; }
}