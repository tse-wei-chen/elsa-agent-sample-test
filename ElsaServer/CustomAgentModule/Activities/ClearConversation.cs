using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using ElsaServer.CustomAgentModule.Services;

namespace ElsaServer.CustomAgentModule.Activities;

/// <summary>
/// Activity for clearing conversation history
/// </summary>
[Activity("CustomAgent", "AI Agents", "Clear conversation history for an agent")]
public class ClearConversation : CodeActivity
{
    private readonly IConversationStore _conversationStore;
    
    public ClearConversation(IConversationStore conversationStore)
    {
        _conversationStore = conversationStore;
    }
    
    /// <summary>
    /// Agent ID
    /// </summary>
    [Input(Description = "Agent identifier")]
    public Input<string> AgentId { get; set; } = default!;
    
    /// <summary>
    /// Conversation ID
    /// </summary>
    [Input(Description = "Conversation identifier")]
    public Input<string> ConversationId { get; set; } = default!;
    
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var agentId = context.Get(AgentId);
        var conversationId = context.Get(ConversationId);
        
        await _conversationStore.ClearConversationAsync(
            agentId ?? string.Empty,
            conversationId ?? string.Empty,
            context.CancellationToken);
    }
}
