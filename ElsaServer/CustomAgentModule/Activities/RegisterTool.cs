using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using ElsaServer.CustomAgentModule.Models;
using ElsaServer.CustomAgentModule.Services;

namespace ElsaServer.CustomAgentModule.Activities;

/// <summary>
/// Activity for registering a tool in the tool registry
/// </summary>
[Activity("CustomAgent", "AI Agents", "Register a tool that agents can use")]
public class RegisterTool : CodeActivity
{
    private readonly IToolRegistry _toolRegistry;
    
    public RegisterTool(IToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry;
    }
    
    /// <summary>
    /// Unique identifier for the tool
    /// </summary>
    [Input(Description = "Unique identifier for the tool")]
    public Input<string> ToolId { get; set; } = default!;
    
    /// <summary>
    /// Name of the tool
    /// </summary>
    [Input(Description = "Name of the tool")]
    public Input<string> ToolName { get; set; } = default!;
    
    /// <summary>
    /// Description of the tool
    /// </summary>
    [Input(Description = "Description of what the tool does", UIHint = "multi-line")]
    public Input<string> Description { get; set; } = default!;
    
    /// <summary>
    /// Type of tool
    /// </summary>
    [Input(
        Description = "Type of tool",
        UIHint = "dropdown",
        Options = new[] { "Custom", "Flow", "Api" })]
    public Input<string> ToolType { get; set; } = new("Custom");
    
    /// <summary>
    /// Workflow definition ID (for Flow type tools)
    /// </summary>
    [Input(Description = "Workflow definition ID (required for Flow type tools)")]
    public Input<string?> WorkflowDefinitionId { get; set; } = default!;
    
    /// <summary>
    /// JSON schema for tool parameters
    /// </summary>
    [Input(Description = "JSON schema describing the tool's parameters", UIHint = "multi-line")]
    public Input<string?> ParametersSchema { get; set; } = default!;
    
    protected override ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var toolId = context.Get(ToolId);
        var toolName = context.Get(ToolName);
        var description = context.Get(Description);
        var toolType = context.Get(ToolType);
        var workflowDefinitionId = context.Get(WorkflowDefinitionId);
        var parametersSchema = context.Get(ParametersSchema);
        
        var tool = new ToolDefinition
        {
            Id = toolId ?? Guid.NewGuid().ToString(),
            Name = toolName ?? string.Empty,
            Description = description ?? string.Empty,
            ToolType = toolType ?? "Custom",
            WorkflowDefinitionId = workflowDefinitionId,
            ParametersSchema = parametersSchema
        };
        
        _toolRegistry.RegisterTool(tool);
        
        return ValueTask.CompletedTask;
    }
}
