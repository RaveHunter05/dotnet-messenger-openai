namespace bot_messenger.Services;

public class ChatService : IChatService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IConfiguration _configuration;
    private readonly IOpenAIService _openAIService;
    private readonly GenericSearchService _genericSearchService;

    public ChatService(
        IEmbeddingService embeddingService,
        IConfiguration configuration,
        IOpenAIService openAIService,
        GenericSearchService genericSearchService
    )
    {
        _embeddingService = embeddingService;
        _configuration = configuration;
        _openAIService = openAIService;
        _genericSearchService = genericSearchService;
    }

    public async Task<string> GetContextAwareResponseAsync(string userMessage)
    {
        // 2. Get List of search
        var similarDocuments = await _genericSearchService.SearchWithFiltersAsync(
            query: userMessage
        );

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
