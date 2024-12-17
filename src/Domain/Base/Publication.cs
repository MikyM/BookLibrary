using Domain.Enums;

// we disable these due to EF handling the private field
// ReSharper disable CollectionNeverUpdated.Local
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value


namespace Domain.Base;

/// <inheritdoc cref="IPublication"/>
/// <inheritdoc cref="SnowflakeEntityBase"/>
/// <remarks>
/// We will use TPC (table-per-concrete-type) EF feature which is available since EF 7.0 and let EF handle inheritance.
/// This is also why we opt for snowflake IDs - the IDs of types in the hierarchy must be unique.
/// </remarks>
public abstract class Publication : SnowflakeEntityBase, IPublication
{
    private HashSet<PublicationAuthor>? _publicationAuthors;
    private HashSet<Author>? _authors;
    
    private HashSet<OrderDetail>? _orderDetails;
    private HashSet<Order>? _orders;
    
    /// <inheritdoc/>
    public IEnumerable<PublicationAuthor>? PublicationAuthors => _publicationAuthors?.AsEnumerable();
    /// <inheritdoc/>
    public IEnumerable<Author>? Authors => _authors?.AsEnumerable();
    
    /// <inheritdoc/>
    public IEnumerable<OrderDetail>? OrderDetails => _orderDetails?.AsEnumerable();
    /// <inheritdoc/>
    public IEnumerable<Order>? Orders => _orders?.AsEnumerable();
    
    /// <inheritdoc/>
    public abstract PublicationType Type { get; }

    /// <inheritdoc/>
    public string Title { get; set; } = string.Empty;
    
    /// <inheritdoc/>
    public decimal Price { get; set; }
    
    /// <inheritdoc/>
    public long Bookstand { get; set; }
    
    /// <inheritdoc/>
    public long Shelf { get; set; }
    
    public void WithAuthors(IEnumerable<Author> authors)
    {
        _authors = authors.ToHashSet();
    }
    
    public void WithAuthor(Author author)
    {
        _authors ??= new HashSet<Author>();
        
        _authors.Add(author);
    }
}