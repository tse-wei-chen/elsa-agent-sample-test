using System.Text;
using System.Text.Json;
using ElsaServer.CustomAgentModule.Models;

namespace ElsaServer.CustomAgentModule.Services;

/// <summary>
/// Model provider for OpenAI-compatible APIs
/// </summary>
public class OpenAIModelProvider : IModelProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OpenAIModelProvider> _logger;
    
    public string ProviderName => "OpenAI";
    
    public OpenAIModelProvider(
        IHttpClientFactory httpClientFactory,
        ILogger<OpenAIModelProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    
    public async Task<string> ExecuteChatAsync(
        AgentConfig config,
        List<ConversationMessage> messages,
        List<ToolDefinition>? tools = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            
            // Determine endpoint - use custom endpoint if provided, otherwise use OpenAI
            var endpoint = config.ApiEndpoint ?? "https://api.openai.com/v1/chat/completions";
            
            // Build request payload
            var requestPayload = new
            {
                model = config.ModelName,
                messages = messages.Select(m => new
                {
                    role = m.Role,
                    content = m.Content
                }),
                temperature = config.Temperature,
                max_tokens = config.MaxTokens
            };
            
            var content = new StringContent(
                JsonSerializer.Serialize(requestPayload),
                Encoding.UTF8,
                "application/json");
            
            // Add authorization header
            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.ApiKey);
            }
            
            var response = await httpClient.PostAsync(endpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonResponse = JsonDocument.Parse(responseContent);
            
            // Extract the assistant's message
            var assistantMessage = jsonResponse.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
            
            return assistantMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling model provider");
            return $"Error: {ex.Message}";
        }
    }
}
