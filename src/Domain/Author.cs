﻿using Domain.Base;

// we disable these due to EF handling the private field
// ReSharper disable CollectionNeverUpdated.Local
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace Domain;

/// <summary>
/// Represents an author.
/// </summary>
public sealed class Author : SnowflakeEntityBase
{
    private HashSet<PublicationAuthor>? _publicationAuthors;
    private HashSet<Publication>? _publications;
    
    /// <summary>
    /// Gets the publication authors collection (join entity).
    /// </summary>
    public IEnumerable<PublicationAuthor>? PublicationAuthors => _publicationAuthors?.AsEnumerable();

    /// <summary>
    /// Gets all publications that were written by this author.
    /// </summary>
    public IEnumerable<Publication>? Publications => _publications?.AsEnumerable();

    /// <summary>
    /// Author's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Author's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
}