using Gridify;

namespace Models;

public class ExtendedPaging<TData> : Paging<TData>, IExtendedPaging<TData> where TData : class
{
    public int TotalPages { get; private set; }
    public int CurrentPage { get; private set; }
    public int PageSize { get; private set; }
    public int? PreviousPage { get; private set; }
    public int? NextPage { get; private set; }

    public ExtendedPaging(int count, int pageSize, int currentPage, IEnumerable<TData> data)
    {
        Count = count;
        Data = data;
        PageSize = pageSize;
        CurrentPage = currentPage;
        
        var totalPages = Count == 0
            ? 1
            : (int)Math.Ceiling((double)Count / PageSize);
        
        TotalPages = totalPages;

        PreviousPage = currentPage <= 1
            ? null
            : currentPage - 1;

        NextPage = currentPage >= totalPages
            ? null
            : currentPage + 1;
    }
}