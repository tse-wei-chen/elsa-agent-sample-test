using ElsaServer.CustomAgentModule.Models;

namespace ElsaServer.CustomAgentModule.Services;

/// <summary>
/// Registry for managing tools
/// </summary>
public interface IToolRegistry
{
    /// <summary>
    /// Register a tool
    /// </summary>
    void RegisterTool(ToolDefinition tool);
    
    /// <summary>
    /// Get a tool by ID
    /// </summary>
    ToolDefinition? GetTool(string toolId);
    
    /// <summary>
    /// Get all registered tools
    /// </summary>
    IEnumerable<ToolDefinition> GetAllTools();
    
    /// <summary>
    /// Get tools by their IDs
    /// </summary>
    IEnumerable<ToolDefinition> GetTools(IEnumerable<string> toolIds);
}
