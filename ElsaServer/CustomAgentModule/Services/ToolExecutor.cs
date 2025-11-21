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
        
        // NOTE: Flow tool execution is designed to execute another Elsa workflow as a tool.
        // The workflow runtime API has evolved in Elsa 3.5, and the exact method for 
        // programmatic workflow execution requires further investigation of the IWorkflowClient API.
        // For production use, implement the actual workflow execution using the appropriate
        // Elsa 3.5 runtime client methods.
        await Task.CompletedTask; // Suppress unused parameter warning
        
        return JsonSerializer.Serialize(new 
        { 
            status = "flow_tool_configured", 
            workflowDefinitionId = tool.WorkflowDefinitionId,
            input = input,
            message = "Flow tool is registered and configured. Workflow execution requires implementation using Elsa 3.5 runtime client API."
        });
    }
    
    private Task<string> ExecuteCustomToolAsync(
        ToolDefinition tool,
        string arguments,
        CancellationToken cancellationToken)
    {
        // NOTE: Custom tool execution is extensible - developers can implement specific
        // tool logic here based on the tool name or other properties.
        // Examples: Calculator, WebSearch, DatabaseQuery, etc.
        // This placeholder allows the module to be extended with custom tool implementations.
        return Task.FromResult($"Custom tool '{tool.Name}' executed with arguments: {arguments}");
    }
}
