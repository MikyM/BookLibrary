using AutoMapper.QueryableExtensions;
using DataAccess;
using DataExplorer.EfCore.Abstractions.DataServices;
using DataExplorer.Entities;
using Gridify;
using Gridify.EntityFramework;
using Models;
using Remora.Results;

namespace Application.Extensions;

public static class ReadOnlyDataServiceExtensions
{
    public static async Task<Result<IExtendedPaging<TEntity>>> GetAsync<TEntity,TId>(this IReadOnlyDataService<TEntity, TId, IBookLibraryDbContext> dataService, 
        IGridifyQuery gridifyQuery, CancellationToken cancellationToken = default) 
        where TEntity : Entity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
    {
        try
        {
            var queryableSet = dataService.ReadOnlyRepository.Set.AsQueryable();
            
            // prevent unexpected results if order by is not present
            if (string.IsNullOrWhiteSpace(gridifyQuery.OrderBy))
            {
                queryableSet = queryableSet.OrderBy(x => x.Id);
            }
            
            var result = await queryableSet.GridifyAsync(gridifyQuery, cancellationToken);

            return new ExtendedPaging<TEntity>(result.Count, gridifyQuery.PageSize, gridifyQuery.Page, result.Data);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public static async Task<Result<IExtendedPaging<TPayloadInterface>>> GetAsync<TEntity,TId,TPayloadImplementation,TPayloadInterface>(this IReadOnlyDataService<TEntity, TId, IBookLibraryDbContext> dataService, 
        IGridifyQuery gridifyQuery, CancellationToken cancellationToken = default) 
        where TEntity : Entity<TId> where TId : IComparable, IEquatable<TId>, IComparable<TId>
        where TPayloadImplementation : TPayloadInterface
        where TPayloadInterface : class
    {
        try
        {
            var queryableSet = dataService.ReadOnlyRepository.Set.AsQueryable();
            
            // prevent unexpected results if order by is not present
            if (string.IsNullOrWhiteSpace(gridifyQuery.OrderBy))
            {
                queryableSet = queryableSet.OrderBy(x => x.Id);
            }

            var queryable = queryableSet.ProjectTo<TPayloadImplementation>(dataService.Mapper.ConfigurationProvider);
            
            var result = await queryable.GridifyAsync(gridifyQuery, cancellationToken);
            
            return new ExtendedPaging<TPayloadInterface>(result.Count, gridifyQuery.PageSize, gridifyQuery.Page, result.Data as IEnumerable<TPayloadInterface> ?? throw new InvalidOperationException("Failed to cast data"));
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}