namespace bot_messenger.Services;

using System.Text;
using bot_messenger.Context;
using Npgsql;

// Services/GenericSearchService.cs
public class GenericSearchService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public GenericSearchService(
        AppDbContext context,
        IEmbeddingService embeddingService,
        IConfiguration configuration
    )
    {
        _embeddingService = embeddingService;
        _context = context;
        _configuration = configuration;
    }

    public async Task<List<VectorSearchResult>> SearchWithFiltersAsync(
        string query,
        string entityType = null,
        Dictionary<string, object> filters = null
    )
    {
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseVector();
        await using var dataSource = dataSourceBuilder.Build();

        var connection = await dataSource.OpenConnectionAsync();
        var sql = new StringBuilder(
            @"
            SELECT 
                id,
                content,
                metadata,
                source_table,
                1 - (embedding <=> @queryEmbedding) as similarity
            FROM document_embeddings
            WHERE 1=1"
        );

        var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter(
                "queryEmbedding",
                NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Real
            )
            {
                Value = queryEmbedding,
            },
        };

        // Filtro por tipo de entidad
        if (!string.IsNullOrEmpty(entityType))
        {
            sql.Append(" AND metadata->>'EntityType' = @entityType");
            parameters.Add(new NpgsqlParameter("entityType", entityType));
        }

        // Filtros adicionales en metadatos
        if (filters != null)
        {
            foreach (var filter in filters)
            {
                var paramName = $"filter_{filter.Key}";
                sql.Append($" AND metadata->>'{filter.Key}' = @{paramName}");
                parameters.Add(new NpgsqlParameter(paramName, filter.Value?.ToString()));
            }
        }

        sql.Append(" ORDER BY embedding <=> @queryEmbedding LIMIT 10");

        using var command = connection.CreateCommand();
        command.CommandText = sql.ToString();
        command.Parameters.AddRange(parameters.ToArray());

        var results = new List<VectorSearchResult>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(
                new VectorSearchResult
                {
                    Id = reader.GetInt32(0),
                    Content = reader.GetString(1),
                    Metadata = reader.GetString(2),
                    SourceTable = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Similarity = reader.GetDouble(4),
                }
            );
        }

        return results;
    }

    // Búsqueda específica por tipo
    public async Task<List<VectorSearchResult>> SearchProductsAsync(
        string query,
        string category = null,
        string location = null
    )
    {
        var filters = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(category))
            filters.Add("Category", category);
        if (!string.IsNullOrEmpty(location))
            filters.Add("Location.City", location);

        return await SearchWithFiltersAsync(query, "Product", filters);
    }

    public async Task<List<VectorSearchResult>> SearchCustomersAsync(
        string query,
        string tier = null,
        string segment = null
    )
    {
        var filters = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(tier))
            filters.Add("Tier", tier);
        if (!string.IsNullOrEmpty(segment))
            filters.Add("Segment", segment);

        return await SearchWithFiltersAsync(query, "Customer", filters);
    }
}

public class VectorSearchResult
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Metadata { get; set; }
    public string SourceTable { get; set; }
    public double Similarity { get; set; }

    public T GetMetadata<T>()
        where T : class
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(Metadata);
    }
}
