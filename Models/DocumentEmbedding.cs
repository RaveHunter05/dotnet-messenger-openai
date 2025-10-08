namespace bot_messenger.Models;

public class DocumentEmbedding
{
    public int Id { get; set; }
    public string SourceTable { get; set; }
    public string Content { get; set; }
    public float[] Embedding { get; set; } // Se mapea al tipo vector
    public string Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
}
