namespace ElsaServer.CustomAgentModule.Models;

/// <summary>
/// A single message in a conversation
/// </summary>
public class ConversationMessage
{
    /// <summary>
    /// Role of the message sender (e.g., "user", "assistant", "system", "tool")
    /// </summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// Content of the message
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the tool that was called (if role is "tool")
    /// </summary>
    public string? ToolName { get; set; }
    
    /// <summary>
    /// Tool call ID (for tracking tool calls and responses)
    /// </summary>
    public string? ToolCallId { get; set; }
    
    /// <summary>
    /// Timestamp of the message
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
