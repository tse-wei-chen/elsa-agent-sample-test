using ElsaServer.CustomAgentModule.Models;

namespace ElsaServer.CustomAgentModule.Services;

/// <summary>
/// Interface for tool execution
/// </summary>
public interface IToolExecutor
{
    /// <summary>
    /// Execute a tool with the given arguments
    /// </summary>
    Task<string> ExecuteToolAsync(
        ToolDefinition tool,
        string arguments,
        CancellationToken cancellationToken = default);
}
