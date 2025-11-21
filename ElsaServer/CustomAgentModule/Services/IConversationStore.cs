using ElsaServer.CustomAgentModule.Models;

namespace ElsaServer.CustomAgentModule.Services;

/// <summary>
/// Service for storing and retrieving conversation history
/// </summary>
public interface IConversationStore
{
    /// <summary>
    /// Store conversation history
    /// </summary>
    Task StoreConversationAsync(
        string agentId,
        string conversationId,
        List<ConversationMessage> messages,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieve conversation history
    /// </summary>
    Task<List<ConversationMessage>> GetConversationAsync(
        string agentId,
        string conversationId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Clear conversation history
    /// </summary>
    Task ClearConversationAsync(
        string agentId,
        string conversationId,
        CancellationToken cancellationToken = default);
}
