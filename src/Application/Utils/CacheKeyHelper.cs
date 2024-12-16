using Domain.Base;

namespace Application.Utils;

public static class CacheKeyHelper
{
    public static string GetCacheKey<TEntity, TId>(TEntity entity) where TEntity : EntityBase<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
        => GetCacheKey<TEntity>(entity.Id.ToString() ?? throw new InvalidOperationException("The ID of the entity is invalid"));
    
    public static string GetCacheKey<TEntity>(string id)
    {
        var type = typeof(TEntity);

        return $"entity:{type.Name.ToLowerInvariant()}:{id}";
    }
}