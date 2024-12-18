using DataExplorer.Abstractions.Entities;
using Domain.Enums;

namespace Domain.Base;

/// <summary>
/// Represents a publication (ie. a book).
/// </summary>
public interface IPublication : ISnowflakeEntity, IEntityBase<long>
{
    /// <summary>
    /// Gets the type of the publication.
    /// </summary>
    PublicationType Type { get; }
    /// <summary>
    /// Gets or sets the title of the publication.
    /// </summary>
    string Title { get; set; }
    
    /// <summary>
    /// Gets the publication authors (the join entity).
    /// </summary>
    public IEnumerable<PublicationAuthor>? PublicationAuthors { get; }
    
    /// <summary>
    /// Gets the authors of this publication.
    /// </summary>
    public IEnumerable<Author>? Authors { get; }
    
    /// <summary>
    /// Gets the order details in which this publication is present.
    /// </summary>
    public IEnumerable<OrderDetail>? OrderDetails { get; }
    
    /// <summary>
    /// Gets the orders in which this publication is present.
    /// </summary>
    public IEnumerable<Order>? Orders { get; }

    /// <summary>
    /// Gets or sets the publication base price.
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="Bookstand"/> on which this publication can be found.
    /// </summary>
    public int Bookstand { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="Shelf"/> on which this publication can be found.
    /// </summary>
    public int Shelf { get; set; }
}