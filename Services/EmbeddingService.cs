using OpenAI.Embeddings;

namespace bot_messenger.Services
{
    public class EmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;
        private readonly string _openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        public EmbeddingService()
        {
            _embeddingClient = new("text-embedding-3-small", apiKey: _openAIApiKey);
        }

        public async Task<float[]> CreateEmbeddingAsync(string text)
        {

            // embedding is basically a group of vectors that represent real world items, such as plants, words, images, etc
            OpenAIEmbedding embedding = await _embeddingClient.GenerateEmbeddingAsync(
                text
            );

            ReadOnlyMemory<float> embeddingVector = embedding.ToFloats();

            return embeddingVector.ToArray();

        }
    }
}
