namespace Models;

public interface IExtendedPaging<TData> where TData : class
{
    int TotalPages { get; }
    int CurrentPage { get; }
    int PageSize { get; }
    int? PreviousPage { get; }
    int? NextPage { get; }
    IEnumerable<TData> Data { get; }
}