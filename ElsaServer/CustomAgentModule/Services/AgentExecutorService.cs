using ElsaServer.CustomAgentModule.Models;

namespace ElsaServer.CustomAgentModule.Services;

/// <summary>
/// Service for executing AI agents
/// </summary>
public class AgentExecutorService
{
    private readonly IEnumerable<IModelProvider> _modelProviders;
    private readonly IToolExecutor _toolExecutor;
    private readonly IToolRegistry _toolRegistry;
    private readonly IConversationStore _conversationStore;
    private readonly ILogger<AgentExecutorService> _logger;
    
    public AgentExecutorService(
        IEnumerable<IModelProvider> modelProviders,
        IToolExecutor toolExecutor,
        IToolRegistry toolRegistry,
        IConversationStore conversationStore,
        ILogger<AgentExecutorService> logger)
    {
        _modelProviders = modelProviders;
        _toolExecutor = toolExecutor;
        _toolRegistry = toolRegistry;
        _conversationStore = conversationStore;
        _logger = logger;
    }
    
    public async Task<AgentResponse> ExecuteAsync(
        AgentConfig config,
        string userMessage,
        string? conversationId = null,
        CancellationToken cancellationToken = default)
    {
        conversationId ??= Guid.NewGuid().ToString();
        var response = new AgentResponse
        {
            ConversationHistory = new List<ConversationMessage>()
        };
        
        try
        {
            // Get the appropriate model provider
            var provider = _modelProviders.FirstOrDefault(p => 
                p.ProviderName.Equals(config.ModelProvider, StringComparison.OrdinalIgnoreCase));
            
            if (provider == null)
            {
                response.Success = false;
                response.Error = $"Model provider '{config.ModelProvider}' not found";
                return response;
            }
            
            // Load conversation history if persistence is enabled
            var messages = new List<ConversationMessage>();
            if (config.PersistHistory)
            {
                messages = await _conversationStore.GetConversationAsync(
                    config.Id,
                    conversationId,
                    cancellationToken);
                
                // Trim history to max turns
                if (messages.Count > config.MaxHistoryTurns * 2)
                {
                    messages = messages.Skip(messages.Count - (config.MaxHistoryTurns * 2)).ToList();
                }
            }
            
            // Add system prompt if not already present
            if (!messages.Any(m => m.Role == "system"))
            {
                messages.Insert(0, new ConversationMessage
                {
                    Role = "system",
                    Content = config.SystemPrompt
                });
            }
            
            // Add user message
            messages.Add(new ConversationMessage
            {
                Role = "user",
                Content = userMessage
            });
            
            // Get available tools
            var tools = config.ToolIds.Any() 
                ? _toolRegistry.GetTools(config.ToolIds).ToList()
                : new List<ToolDefinition>();
            
            // Execute agent with potential tool calls (max 5 iterations)
            const int maxIterations = 5;
            for (int i = 0; i < maxIterations; i++)
            {
                response.Iterations++;
                
                // Call the model
                var assistantResponse = await provider.ExecuteChatAsync(
                    config,
                    messages,
                    tools,
                    cancellationToken);
                
                // Add assistant response to messages
                messages.Add(new ConversationMessage
                {
                    Role = "assistant",
                    Content = assistantResponse
                });
                
                // Check if the response contains tool calls (simplified - in real implementation would parse structured output)
                // For this example, we'll just return the response
                response.Response = assistantResponse;
                break;
            }
            
            response.ConversationHistory = messages;
            
            // Persist conversation if enabled
            if (config.PersistHistory)
            {
                await _conversationStore.StoreConversationAsync(
                    config.Id,
                    conversationId,
                    messages,
                    cancellationToken);
            }
            
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing agent");
            response.Success = false;
            response.Error = ex.Message;
        }
        
        return response;
    }
}
