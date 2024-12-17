using Domain.Enums;

namespace Domain;

using Publication = Base.Publication;

public sealed class Magazine : Publication
{
    /// <inheritdoc/>
    public override PublicationType Type => PublicationType.Magazine;
}