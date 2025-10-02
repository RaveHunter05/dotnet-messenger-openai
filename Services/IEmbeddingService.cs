using bot_messenger.Models;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
    Task<List<DocumentEmbedding>> FindSimilarDocumentsAsync(string query, int limit = 5);
    Task StoreEmbeddingAsync(string content, float[] embedding, string metadata = null);
}
