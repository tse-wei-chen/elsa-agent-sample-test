# Custom AI Agent Module for Elsa Workflows

This module provides AI agent functionality for Elsa Workflows, similar to n8n's agent features. It allows you to create intelligent workflow agents with tool support, model selection, conversation persistence, and the ability to use other workflows as tools.

## Features

### 1. **AI Agent Execution**
- Execute AI agents with configurable system prompts
- Support for multiple AI model providers
- Conversation history management
- Multi-turn conversations with context

### 2. **Model Provider Support**
- **OpenAI**: GPT-3.5, GPT-4, and other OpenAI models
- **Ollama**: Local models (llama2, mistral, etc.)
- **Extensible**: Easy to add new providers

### 3. **Tool System**
- Register custom tools for agents to use
- **Flow Tools**: Use any Elsa workflow as a tool
- Tool parameter schemas
- Tool execution tracking

### 4. **Persistence**
- Conversation history storage
- Configurable history length
- Agent configuration persistence

## Components

### Models

- **AgentConfig**: Configuration for an AI agent including model settings, tools, and prompts
- **ToolDefinition**: Definition of a tool that can be used by agents
- **ConversationMessage**: Individual messages in a conversation
- **AgentResponse**: Response from agent execution with metadata

### Services

- **IModelProvider**: Interface for AI model providers
  - OpenAIModelProvider: OpenAI and compatible APIs
  - OllamaModelProvider: Local model support via Ollama
  
- **IToolExecutor**: Executes tools called by agents
- **IToolRegistry**: Manages registered tools
- **IConversationStore**: Stores and retrieves conversation history
- **AgentExecutorService**: Main service for executing agents

### Activities

#### AgentExecutor
Main activity for executing an AI agent in a workflow.

**Inputs:**
- `UserMessage`: The message to send to the agent
- `AgentName`: Name of the agent
- `SystemPrompt`: System prompt defining agent behavior
- `ModelProvider`: Provider to use (OpenAI, Ollama)
- `ModelName`: Specific model (e.g., gpt-4, llama2)
- `ApiEndpoint`: Optional API endpoint (for local models)
- `ApiKey`: API key for the provider
- `Temperature`: Response creativity (0.0-1.0)
- `MaxTokens`: Maximum response length
- `ToolIds`: Comma-separated list of tool IDs
- `ConversationId`: ID for maintaining conversation context
- `PersistHistory`: Whether to save conversation history
- `MaxHistoryTurns`: Maximum history length

**Outputs:**
- `Response`: The agent's text response
- `FullResponse`: Complete response object with metadata

#### RegisterTool
Register a tool that agents can use.

**Inputs:**
- `ToolId`: Unique identifier
- `ToolName`: Tool name
- `Description`: What the tool does
- `ToolType`: Type (Custom, Flow, Api)
- `WorkflowDefinitionId`: For Flow tools - the workflow to execute
- `ParametersSchema`: JSON schema for parameters

#### ClearConversation
Clear conversation history for an agent.

**Inputs:**
- `AgentId`: Agent identifier
- `ConversationId`: Conversation identifier

## Usage Examples

### Example 1: Simple AI Assistant

```
1. Add AgentExecutor activity
2. Configure:
   - UserMessage: "What is the capital of France?"
   - ModelProvider: "OpenAI"
   - ModelName: "gpt-3.5-turbo"
   - ApiKey: "<your-api-key>"
   - SystemPrompt: "You are a helpful geography expert."
```

### Example 2: Local Model with Ollama

```
1. Install Ollama locally and pull a model (e.g., llama2)
2. Add AgentExecutor activity
3. Configure:
   - UserMessage: "Explain quantum computing"
   - ModelProvider: "Ollama"
   - ModelName: "llama2"
   - ApiEndpoint: "http://localhost:11434/api/chat"
   - SystemPrompt: "You are a physics expert."
```

### Example 3: Agent with Tools

```
1. Create a workflow that performs a calculation (e.g., workflow ID: "calc-workflow")
2. Add RegisterTool activity:
   - ToolId: "calculator"
   - ToolName: "Calculator"
   - Description: "Performs mathematical calculations"
   - ToolType: "Flow"
   - WorkflowDefinitionId: "calc-workflow"

3. Add AgentExecutor activity:
   - UserMessage: "Calculate 15 * 23"
   - ToolIds: "calculator"
   - ModelProvider: "OpenAI"
   - ApiKey: "<your-api-key>"
```

### Example 4: Multi-turn Conversation

```
1. First AgentExecutor:
   - UserMessage: "Tell me about Paris"
   - ConversationId: "paris-conversation"
   - PersistHistory: true

2. Second AgentExecutor (same workflow or triggered later):
   - UserMessage: "What's the weather like there?"
   - ConversationId: "paris-conversation"
   - PersistHistory: true
   
The agent will remember the context about Paris!
```

### Example 5: Using Another Workflow as a Tool

```
1. Create a "Weather Lookup" workflow:
   - Input: city (string)
   - Output: weather data
   
2. Register it as a tool:
   - ToolId: "weather-tool"
   - ToolName: "WeatherLookup"
   - ToolType: "Flow"
   - WorkflowDefinitionId: "<weather-workflow-id>"
   - ParametersSchema: {"type": "object", "properties": {"city": {"type": "string"}}}

3. Use in agent:
   - UserMessage: "What's the weather in Tokyo?"
   - ToolIds: "weather-tool"
```

## Configuration

### OpenAI Configuration
```
ModelProvider: "OpenAI"
ModelName: "gpt-3.5-turbo" or "gpt-4"
ApiKey: Your OpenAI API key
ApiEndpoint: (optional) Custom endpoint for OpenAI-compatible APIs
```

### Ollama Configuration
```
ModelProvider: "Ollama"
ModelName: Model name (e.g., "llama2", "mistral", "codellama")
ApiEndpoint: "http://localhost:11434/api/chat" (or your Ollama server URL)
ApiKey: Not required for Ollama
```

## Implementation Details

### Architecture

```
┌─────────────────────────────────────────┐
│         AgentExecutor Activity          │
│  (Elsa Workflow Activity)               │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│     AgentExecutorService                │
│  - Manages conversation flow            │
│  - Coordinates model & tools            │
└──────────┬──────────────────────────────┘
           │
           ├──────────┬──────────────┬─────┐
           ▼          ▼              ▼     ▼
    ┌──────────┐ ┌─────────┐  ┌────────┐ ┌──────────────┐
    │  Model   │ │  Tool   │  │  Tool  │ │Conversation  │
    │ Provider │ │Registry │  │Executor│ │    Store     │
    └──────────┘ └─────────┘  └────────┘ └──────────────┘
```

### Extensibility

#### Adding a New Model Provider

1. Implement `IModelProvider` interface
2. Register in `CustomAgentExtensions.cs`

Example:
```csharp
public class MyCustomProvider : IModelProvider
{
    public string ProviderName => "MyProvider";
    
    public async Task<string> ExecuteChatAsync(
        AgentConfig config,
        List<ConversationMessage> messages,
        List<ToolDefinition>? tools = null,
        CancellationToken cancellationToken = default)
    {
        // Your implementation
    }
}

// In CustomAgentExtensions.cs:
module.Services.AddScoped<IModelProvider, MyCustomProvider>();
```

## Comparison with n8n Agent

| Feature | This Module | n8n Agent |
|---------|-------------|-----------|
| Multiple Model Providers | ✅ | ✅ |
| Local Model Support | ✅ | ✅ |
| Custom Tools | ✅ | ✅ |
| Flow as Tool | ✅ | ✅ |
| Conversation Persistence | ✅ | ✅ |
| Tool Parameter Schema | ✅ | ✅ |
| UI Configuration | ✅ (via Elsa Studio) | ✅ |

## Future Enhancements

- [ ] Advanced tool calling with structured output parsing
- [ ] Streaming responses
- [ ] Token usage tracking and limits
- [ ] Agent memory systems (vector databases)
- [ ] Multi-agent coordination
- [ ] Enhanced error handling and retries
- [ ] Tool call visualization
- [ ] Pre-built tool library

## License

This module follows the same license as the parent project.
