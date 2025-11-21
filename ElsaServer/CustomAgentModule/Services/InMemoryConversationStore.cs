using System.Collections.Concurrent;
using ElsaServer.CustomAgentModule.Models;

namespace ElsaServer.CustomAgentModule.Services;

/// <summary>
/// In-memory implementation of conversation store
/// </summary>
public class InMemoryConversationStore : IConversationStore
{
    private readonly ConcurrentDictionary<string, List<ConversationMessage>> _conversations = new();
    
    private static string GetKey(string agentId, string conversationId)
    {
        return $"{agentId}:{conversationId}";
    }
    
    public Task StoreConversationAsync(
        string agentId,
        string conversationId,
        List<ConversationMessage> messages,
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(agentId, conversationId);
        _conversations[key] = new List<ConversationMessage>(messages);
        return Task.CompletedTask;
    }
    
    public Task<List<ConversationMessage>> GetConversationAsync(
        string agentId,
        string conversationId,
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(agentId, conversationId);
        var messages = _conversations.TryGetValue(key, out var msgs) 
            ? new List<ConversationMessage>(msgs) 
            : new List<ConversationMessage>();
        return Task.FromResult(messages);
    }
    
    public Task ClearConversationAsync(
        string agentId,
        string conversationId,
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(agentId, conversationId);
        _conversations.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
