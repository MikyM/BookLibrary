using Domain.Base;
using Domain.Enums;

namespace Domain;

/// <summary>
/// Represents a book.
/// </summary>
/// <inheritdoc cref="Publication"/>
public sealed class Book : Publication, IPublication
{
    /// <inheritdoc/>
    public override PublicationType Type => PublicationType.Book;
}