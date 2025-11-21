using System.Text.Json;
using ElsaServer.CustomAgentModule.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Management;

namespace ElsaServer.CustomAgentModule.Services;

/// <summary>
/// Tool executor that can execute custom tools and workflow-based tools
/// </summary>
public class ToolExecutor : IToolExecutor
{
    private readonly IWorkflowRuntime _workflowRuntime;
    private readonly IWorkflowDefinitionService _workflowDefinitionService;
    private readonly ILogger<ToolExecutor> _logger;
    
    public ToolExecutor(
        IWorkflowRuntime workflowRuntime,
        IWorkflowDefinitionService workflowDefinitionService,
        ILogger<ToolExecutor> logger)
    {
        _workflowRuntime = workflowRuntime;
        _workflowDefinitionService = workflowDefinitionService;
        _logger = logger;
    }
    
    public async Task<string> ExecuteToolAsync(
        ToolDefinition tool,
        string arguments,
        CancellationToken cancellationToken = default)
    {
        try
        {
            switch (tool.ToolType)
            {
                case "Flow":
                    return await ExecuteFlowToolAsync(tool, arguments, cancellationToken);
                    
                case "Custom":
                    return await ExecuteCustomToolAsync(tool, arguments, cancellationToken);
                    
                default:
                    return $"Unknown tool type: {tool.ToolType}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tool {ToolName}", tool.Name);
            return $"Error executing tool: {ex.Message}";
        }
    }
    
    private async Task<string> ExecuteFlowToolAsync(
        ToolDefinition tool,
        string arguments,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(tool.WorkflowDefinitionId))
        {
            return "No workflow definition ID specified for flow tool";
        }
        
        // Parse arguments into input dictionary
        var input = new Dictionary<string, object>();
        try
        {
            var argsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(arguments);
            if (argsDict != null)
            {
                input = argsDict;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse tool arguments, using empty input");
        }
        
        // TODO: Execute the workflow using the Elsa workflow runtime
        // For now, return a placeholder indicating that flow tool execution is configured
        await Task.CompletedTask; // Suppress unused parameter warning
        
        return JsonSerializer.Serialize(new 
        { 
            status = "flow_tool_executed", 
            workflowDefinitionId = tool.WorkflowDefinitionId,
            input = input,
            message = "Flow tool execution is configured. Implementation requires workflow runtime integration."
        });
    }
    
    private Task<string> ExecuteCustomToolAsync(
        ToolDefinition tool,
        string arguments,
        CancellationToken cancellationToken)
    {
        // For custom tools, you would implement specific logic here
        // For now, return a placeholder
        return Task.FromResult($"Custom tool {tool.Name} executed with arguments: {arguments}");
    }
}
