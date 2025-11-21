using Elsa.Features.Services;
using ElsaServer.CustomAgentModule.Services;

namespace ElsaServer.CustomAgentModule.Extensions;

/// <summary>
/// Extension methods for registering the Custom Agent module
/// </summary>
public static class CustomAgentExtensions
{
    /// <summary>
    /// Add Custom Agent module to Elsa
    /// </summary>
    public static IModule UseCustomAgents(this IModule module)
    {
        // Register services
        module.Services.AddSingleton<IToolRegistry, InMemoryToolRegistry>();
        module.Services.AddSingleton<IConversationStore, InMemoryConversationStore>();
        module.Services.AddScoped<IToolExecutor, ToolExecutor>();
        module.Services.AddScoped<IModelProvider, OpenAIModelProvider>();
        module.Services.AddScoped<IModelProvider, OllamaModelProvider>();
        module.Services.AddScoped<AgentExecutorService>();
        
        // Register HttpClient for model providers
        module.Services.AddHttpClient();
        
        return module;
    }
}
