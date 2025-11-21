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
            // NOTE: Advanced tool calling requires parsing structured output from the AI model
            // (e.g., function calling format from OpenAI). This simplified version demonstrates
            // the conversation flow. For production, implement:
            // 1. Parse tool calls from AI response
            // 2. Execute requested tools via IToolExecutor
            // 3. Add tool results to conversation
            // 4. Continue iteration until final answer or max iterations
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
                
                // For this foundational implementation, return the response directly
                // Production use would check for tool calls and iterate as needed
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
