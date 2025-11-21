using System.Text;
using System.Text.Json;
using ElsaServer.CustomAgentModule.Models;

namespace ElsaServer.CustomAgentModule.Services;

/// <summary>
/// Model provider for local models via Ollama
/// </summary>
public class OllamaModelProvider : IModelProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OllamaModelProvider> _logger;
    
    public string ProviderName => "Ollama";
    
    public OllamaModelProvider(
        IHttpClientFactory httpClientFactory,
        ILogger<OllamaModelProvider> logger)
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
            
            // Default Ollama endpoint
            var endpoint = config.ApiEndpoint ?? "http://localhost:11434/api/chat";
            
            // Build request payload for Ollama
            var requestPayload = new
            {
                model = config.ModelName,
                messages = messages.Select(m => new
                {
                    role = m.Role,
                    content = m.Content
                }),
                stream = false,
                options = new
                {
                    temperature = config.Temperature,
                    num_predict = config.MaxTokens
                }
            };
            
            var content = new StringContent(
                JsonSerializer.Serialize(requestPayload),
                Encoding.UTF8,
                "application/json");
            
            var response = await httpClient.PostAsync(endpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonResponse = JsonDocument.Parse(responseContent);
            
            // Extract the assistant's message
            var assistantMessage = jsonResponse.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
            
            return assistantMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Ollama");
            return $"Error: {ex.Message}";
        }
    }
}
