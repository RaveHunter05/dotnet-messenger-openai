namespace bot_messenger.Services;

public class ChatService : IChatService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IConfiguration _configuration;
    private readonly IOpenAIService _openAIService;


    public ChatService(IEmbeddingService embeddingService, IConfiguration configuration, IOpenAIService openAIService)
    {
        _embeddingService = embeddingService;
        _configuration = configuration;
        _openAIService = openAIService;
    }

    public async Task<string> GetContextAwareResponseAsync(string userMessage)
    {
        // 1. Buscar documentos similares
        var similarDocuments = await _embeddingService.FindSimilarDocumentsAsync(userMessage, 3);

        // 2. Construir contexto
        var context = string.Join("\n\n", similarDocuments.Select(d => d.Content));

        // 2. Prepare prompt

        var prompt = $"""
        Basado en el siguiente contexto:
        
        {context}
        
        Pregunta del usuario: {userMessage}
        
        Por favor, proporciona una respuesta útil basada en el contexto proporcionado.
        """;

        return await _openAIService.CompleteChat(prompt);
    }
}
