namespace Models.Author;

public record AuthorPayload(string FirstName, string LastName) : IAuthorPayload;