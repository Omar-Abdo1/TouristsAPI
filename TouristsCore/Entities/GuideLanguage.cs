namespace TouristsCore.Entities;

public class GuideLanguage
{
    public int GuideProfileId { get; set; }
    public GuideProfile GuideProfile { get; set; }

    public int LanguageId { get; set; }
    public Language Language { get; set; }
}