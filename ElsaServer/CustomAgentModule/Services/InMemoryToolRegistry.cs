using System.Collections.Concurrent;
using ElsaServer.CustomAgentModule.Models;

namespace ElsaServer.CustomAgentModule.Services;

/// <summary>
/// In-memory implementation of tool registry
/// </summary>
public class InMemoryToolRegistry : IToolRegistry
{
    private readonly ConcurrentDictionary<string, ToolDefinition> _tools = new();
    
    public void RegisterTool(ToolDefinition tool)
    {
        _tools[tool.Id] = tool;
    }
    
    public ToolDefinition? GetTool(string toolId)
    {
        return _tools.TryGetValue(toolId, out var tool) ? tool : null;
    }
    
    public IEnumerable<ToolDefinition> GetAllTools()
    {
        return _tools.Values;
    }
    
    public IEnumerable<ToolDefinition> GetTools(IEnumerable<string> toolIds)
    {
        return toolIds.Select(id => GetTool(id)).Where(t => t != null).Cast<ToolDefinition>();
    }
}
