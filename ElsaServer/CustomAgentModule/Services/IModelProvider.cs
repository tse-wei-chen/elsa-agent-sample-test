using ElsaServer.CustomAgentModule.Models;

namespace ElsaServer.CustomAgentModule.Services;

/// <summary>
/// Interface for AI model providers
/// </summary>
public interface IModelProvider
{
    /// <summary>
    /// Name of the provider
    /// </summary>
    string ProviderName { get; }
    
    /// <summary>
    /// Execute a chat completion with the model
    /// </summary>
    Task<string> ExecuteChatAsync(
        AgentConfig config,
        List<ConversationMessage> messages,
        List<ToolDefinition>? tools = null,
        CancellationToken cancellationToken = default);
}
