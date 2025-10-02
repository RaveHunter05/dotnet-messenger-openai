public interface IChatService
{
    Task<string> GetContextAwareResponseAsync(string userMessage);
}
