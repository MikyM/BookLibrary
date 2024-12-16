using DataExplorer.Abstractions.Entities;
using Domain.Base;

namespace Domain;

/// <summary>
/// Represents a join entity between any given publication and any given author.
/// </summary>
public sealed class PublicationAuthor : ICreatedAtOffset, IUpdatedAtOffset, IValueObject<PublicationAuthor>
{
    public PublicationAuthor(long publicationId, long authorId)
    {
        PublicationId = publicationId;
        AuthorId = authorId;
    }

    public long PublicationId { get; }
    public Publication? Publication { get; set; }

    public long AuthorId { get; }
    public Author? Author { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public bool Equals(PublicationAuthor? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return PublicationId == other.PublicationId && AuthorId == other.AuthorId;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is PublicationAuthor other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PublicationId, AuthorId);
    }
}