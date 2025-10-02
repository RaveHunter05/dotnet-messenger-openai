using OpenAI.Chat;

namespace bot_messenger.Services
{
    public class OpenAIService : IOpenAIService
    {

        private readonly ChatClient _chatClient;

        private readonly string _openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        public OpenAIService(IConfiguration configuration)
        {
            var model = "gpt-4o";
            _chatClient = new ChatClient(model, apiKey: _openAIApiKey);
        }

        public Task<string> CompleteChat(string message)
        {
            ChatCompletion completion = _chatClient.CompleteChat(message);
            return Task.FromResult(completion.Content[0].Text);
        }
    }
}


