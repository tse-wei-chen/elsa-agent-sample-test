namespace ElsaServer.CustomAgentModule.Models;

/// <summary>
/// Definition of a tool that can be used by an agent
/// </summary>
public class ToolDefinition
{
    /// <summary>
    /// Unique identifier for the tool
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Name of the tool
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of what the tool does
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of tool (e.g., "Custom", "Flow", "Api")
    /// </summary>
    public string ToolType { get; set; } = "Custom";
    
    /// <summary>
    /// For Flow type tools - the workflow definition ID to execute
    /// </summary>
    public string? WorkflowDefinitionId { get; set; }
    
    /// <summary>
    /// Parameter schema in JSON format
    /// </summary>
    public string? ParametersSchema { get; set; }
}
