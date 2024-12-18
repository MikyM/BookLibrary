using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace DataAccess;

/// <summary>
/// This had to be implemented due to weird PGSQL behavior @ new version.
/// </summary>
[UsedImplicitly]
[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
public class MigrationHistory : NpgsqlHistoryRepository
{
    public MigrationHistory(HistoryRepositoryDependencies dependencies) : base(dependencies)
    {
    }

    protected override bool InterpretExistsResult(object? value)
    {
        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind == JsonValueKind.True;
        }
        
        return base.InterpretExistsResult(value);
    }
}