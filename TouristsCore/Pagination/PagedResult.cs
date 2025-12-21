namespace TouristsService.Pagination;

public class PagedResult<T>
{
    public List<T>items { get; set; }
    public object?NextCursor { get; set; }
    public bool hasMore { get; set; }
}