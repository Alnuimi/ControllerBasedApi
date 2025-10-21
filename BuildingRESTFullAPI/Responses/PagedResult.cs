using System;

namespace BuildingRESTFullAPI.Responses;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } 
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    
    private PagedResult() { }
    public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int pageSize, int currentPage)
    {
        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = currentPage
        };
    }
}
