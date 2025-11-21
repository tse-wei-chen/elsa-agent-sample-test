using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using ElsaServer.CustomAgentModule.Models;
using ElsaServer.CustomAgentModule.Services;

namespace ElsaServer.CustomAgentModule.Activities;

/// <summary>
/// Activity for executing an AI agent
/// </summary>
[Activity("CustomAgent", "AI Agents", "Execute an AI agent with tools and model selection")]
public class AgentExecutor : CodeActivity<AgentResponse>
{
    private readonly AgentExecutorService _agentExecutorService;
    
    public AgentExecutor(AgentExecutorService agentExecutorService)
    {
        _agentExecutorService = agentExecutorService;
    }
    
    /// <summary>
    /// The user message to send to the agent
    /// </summary>
    [Input(Description = "The user message to send to the agent")]
    public Input<string> UserMessage { get; set; } = default!;
    
    /// <summary>
    /// Agent name/identifier
    /// </summary>
    [Input(Description = "Name of the agent")]
    public Input<string> AgentName { get; set; } = new("AI Assistant");
    
    /// <summary>
    /// System prompt for the agent
    /// </summary>
    [Input(
        Description = "System prompt that defines the agent's behavior and personality",
        UIHint = "multi-line")]
    public Input<string> SystemPrompt { get; set; } = new("You are a helpful AI assistant.");
    
    /// <summary>
    /// Model provider (OpenAI, Ollama, etc.)
    /// </summary>
    [Input(
        Description = "Model provider to use",
        UIHint = "dropdown",
        Options = new[] { "OpenAI", "Ollama" })]
    public Input<string> ModelProvider { get; set; } = new("OpenAI");
    
    /// <summary>
    /// Model name
    /// </summary>
    [Input(Description = "Name of the model to use (e.g., gpt-4, llama2, mistral)")]
    public Input<string> ModelName { get; set; } = new("gpt-3.5-turbo");
    
    /// <summary>
    /// API endpoint (for local models)
    /// </summary>
    [Input(Description = "API endpoint URL (optional, for local models)")]
    public Input<string?> ApiEndpoint { get; set; } = default!;
    
    /// <summary>
    /// API key
    /// </summary>
    [Input(Description = "API key for the model provider")]
    public Input<string?> ApiKey { get; set; } = default!;
    
    /// <summary>
    /// Temperature (0.0 to 1.0)
    /// </summary>
    [Input(Description = "Temperature for response generation (0.0 to 1.0)")]
    public Input<double> Temperature { get; set; } = new(0.7);
    
    /// <summary>
    /// Maximum tokens
    /// </summary>
    [Input(Description = "Maximum tokens for the response")]
    public Input<int> MaxTokens { get; set; } = new(2000);
    
    /// <summary>
    /// Tool IDs to make available to the agent
    /// </summary>
    [Input(Description = "List of tool IDs available to the agent (comma-separated)")]
    public Input<string?> ToolIds { get; set; } = default!;
    
    /// <summary>
    /// Conversation ID for maintaining context
    /// </summary>
    [Input(Description = "Conversation ID to maintain context across multiple calls")]
    public Input<string?> ConversationId { get; set; } = default!;
    
    /// <summary>
    /// Whether to persist conversation history
    /// </summary>
    [Input(Description = "Enable conversation history persistence")]
    public Input<bool> PersistHistory { get; set; } = new(true);
    
    /// <summary>
    /// Maximum conversation history turns
    /// </summary>
    [Input(Description = "Maximum number of conversation turns to keep in history")]
    public Input<int> MaxHistoryTurns { get; set; } = new(10);
    
    /// <summary>
    /// The response from the agent
    /// </summary>
    [Output(Description = "The agent's response")]
    public Output<string> Response { get; set; } = default!;
    
    /// <summary>
    /// Full agent response object
    /// </summary>
    [Output(Description = "Complete agent response with metadata")]
    public Output<AgentResponse> FullResponse { get; set; } = default!;
    
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var userMessage = context.Get(UserMessage);
        var agentName = context.Get(AgentName);
        var systemPrompt = context.Get(SystemPrompt);
        var modelProvider = context.Get(ModelProvider);
        var modelName = context.Get(ModelName);
        var apiEndpoint = context.Get(ApiEndpoint);
        var apiKey = context.Get(ApiKey);
        var temperature = context.Get(Temperature);
        var maxTokens = context.Get(MaxTokens);
        var toolIdsString = context.Get(ToolIds);
        var conversationId = context.Get(ConversationId);
        var persistHistory = context.Get(PersistHistory);
        var maxHistoryTurns = context.Get(MaxHistoryTurns);
        
        // Parse tool IDs
        var toolIds = string.IsNullOrWhiteSpace(toolIdsString)
            ? new List<string>()
            : toolIdsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => id.Trim())
                .ToList();
        
        // Create agent config
        var config = new AgentConfig
        {
            Name = agentName ?? "AI Assistant",
            SystemPrompt = systemPrompt ?? "You are a helpful AI assistant.",
            ModelProvider = modelProvider ?? "OpenAI",
            ModelName = modelName ?? "gpt-3.5-turbo",
            ApiEndpoint = apiEndpoint,
            ApiKey = apiKey,
            Temperature = temperature,
            MaxTokens = maxTokens,
            ToolIds = toolIds,
            PersistHistory = persistHistory,
            MaxHistoryTurns = maxHistoryTurns
        };
        
        // Execute the agent
        var agentResponse = await _agentExecutorService.ExecuteAsync(
            config,
            userMessage ?? string.Empty,
            conversationId,
            context.CancellationToken);
        
        // Set outputs
        context.Set(Response, agentResponse.Response);
        context.Set(FullResponse, agentResponse);
        context.Set(Result, agentResponse);
    }
}
