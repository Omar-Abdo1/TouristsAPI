namespace TouristsAPI.Helpers;

public class Pagination<T> where T:class
{
    public Pagination(int pageSize, int pageIndex, int count, IReadOnlyList<T>? data)
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
        Count = count;
        Data = data;
    }

    public int PageSize { get; set; } = 5;
    public int PageIndex { get; set; } = 1;
    public int Count { get; set; }
    public IReadOnlyList<T>? Data{ get; set; }
}