namespace ElsaServer.CustomAgentModule.Models;

/// <summary>
/// Response from an agent execution
/// </summary>
public class AgentResponse
{
    /// <summary>
    /// The final response text from the agent
    /// </summary>
    public string Response { get; set; } = string.Empty;
    
    /// <summary>
    /// List of tools that were called during execution
    /// </summary>
    public List<ToolCall> ToolCalls { get; set; } = new();
    
    /// <summary>
    /// Complete conversation history
    /// </summary>
    public List<ConversationMessage> ConversationHistory { get; set; } = new();
    
    /// <summary>
    /// Number of iterations/turns taken
    /// </summary>
    public int Iterations { get; set; }
    
    /// <summary>
    /// Total tokens used (if available)
    /// </summary>
    public int? TotalTokens { get; set; }
    
    /// <summary>
    /// Whether the agent successfully completed
    /// </summary>
    public bool Success { get; set; } = true;
    
    /// <summary>
    /// Error message if execution failed
    /// </summary>
    public string? Error { get; set; }
}

/// <summary>
/// Represents a tool call made by the agent
/// </summary>
public class ToolCall
{
    /// <summary>
    /// Name of the tool
    /// </summary>
    public string ToolName { get; set; } = string.Empty;
    
    /// <summary>
    /// Arguments passed to the tool
    /// </summary>
    public string Arguments { get; set; } = string.Empty;
    
    /// <summary>
    /// Result from the tool execution
    /// </summary>
    public string Result { get; set; } = string.Empty;
}
