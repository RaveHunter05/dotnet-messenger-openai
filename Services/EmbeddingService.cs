using OpenAI.Embeddings;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using bot_messenger.Context;
using bot_messenger.Models;

namespace bot_messenger.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;
        private readonly string _openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public EmbeddingService(AppDbContext context, IConfiguration configuration)
        {
            _embeddingClient = new("text-embedding-3-small", apiKey: _openAIApiKey);
            _context = context;
            _configuration = configuration;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {

            // embedding is basically a group of vectors that represent real world items, such as plants, words, images, etc
            OpenAIEmbedding embedding = await _embeddingClient.GenerateEmbeddingAsync(
                text
            );

            ReadOnlyMemory<float> embeddingVector = embedding.ToFloats();

            return embeddingVector.ToArray();

        }

        // For saving generated embedding into the database in to the database
        public async Task StoreEmbeddingAsync(string content, float[] embedding, string metadata = null)
        {
            var document = new DocumentEmbedding
            {
                Content = content,
                Embedding = embedding,
                Metadata = metadata,
                CreatedAt = DateTime.UtcNow
            };

            _context.DocumentEmbeddings.Add(document);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DocumentEmbedding>> FindSimilarDocumentsAsync(string query, int limit = 5)
        {
            // Generar embedding para la consulta
            var queryEmbedding = await GenerateEmbeddingAsync(query);

            // Usar SQL raw para búsqueda vectorial
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var sql = @"
            SELECT id, content, metadata, created_at,
                   embedding <=> @queryEmbedding as distance
            FROM document_embeddings
            ORDER BY embedding <=> @queryEmbedding
            LIMIT @limit";

            var results = new List<DocumentEmbedding>();

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("queryEmbedding", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Real, queryEmbedding);
            command.Parameters.AddWithValue("limit", limit);

            using var reader = await command.ExecuteReaderAsync();

            // avoid using whiles
            while (await reader.ReadAsync())
            {
                results.Add(new DocumentEmbedding
                {
                    Id = reader.GetInt32(0),
                    Content = reader.GetString(1),
                    Metadata = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                    // La distancia está en reader.GetDouble(4) si la necesitas
                });
            }

            return results;
        }
    }
}
