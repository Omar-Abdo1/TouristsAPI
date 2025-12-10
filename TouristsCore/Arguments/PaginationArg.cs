namespace TouristsAPI.Helpers;

public class PaginationArg
{
    const int MaxPageSize = 10;
    
    private int _pageSize = 5;
    private int _pageIndex = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = int.Min(MaxPageSize,value);
    }

    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = int.Max(1,value);
    }
}