namespace TouristsCore.Entities;

public class Language : BaseEntity
{
    public string Name { get; set; }
    public ICollection<GuideLanguage> GuideLanguages { get; set; }
}