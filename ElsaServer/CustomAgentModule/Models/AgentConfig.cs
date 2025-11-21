namespace ElsaServer.CustomAgentModule.Models;

/// <summary>
/// Configuration for an AI agent
/// </summary>
public class AgentConfig
{
    /// <summary>
    /// Unique identifier for the agent
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Name of the agent
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// System prompt for the agent
    /// </summary>
    public string SystemPrompt { get; set; } = string.Empty;
    
    /// <summary>
    /// Model provider (e.g., "OpenAI", "Ollama", "LMStudio")
    /// </summary>
    public string ModelProvider { get; set; } = "OpenAI";
    
    /// <summary>
    /// Model name (e.g., "gpt-4", "llama2", "mistral")
    /// </summary>
    public string ModelName { get; set; } = "gpt-3.5-turbo";
    
    /// <summary>
    /// API endpoint for the model (for local models)
    /// </summary>
    public string? ApiEndpoint { get; set; }
    
    /// <summary>
    /// API key for the model provider
    /// </summary>
    public string? ApiKey { get; set; }
    
    /// <summary>
    /// Temperature for response generation (0.0 to 1.0)
    /// </summary>
    public double Temperature { get; set; } = 0.7;
    
    /// <summary>
    /// Maximum tokens for response
    /// </summary>
    public int MaxTokens { get; set; } = 2000;
    
    /// <summary>
    /// List of tool IDs available to this agent
    /// </summary>
    public List<string> ToolIds { get; set; } = new();
    
    /// <summary>
    /// Whether to persist conversation history
    /// </summary>
    public bool PersistHistory { get; set; } = true;
    
    /// <summary>
    /// Maximum number of conversation turns to keep in history
    /// </summary>
    public int MaxHistoryTurns { get; set; } = 10;
}
