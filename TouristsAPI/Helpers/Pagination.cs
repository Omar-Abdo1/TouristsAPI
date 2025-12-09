namespace TouristsAPI.Helpers;

public class Pagination<T> where T:class
{
    public int PageSize { get; set; } = 5;
    public int PageIndex { get; set; } = 1;
    public int Count { get; set; }
    public IReadOnlyList<T>? Data{ get; set; }
}