# Example Workflows for Custom AI Agent Module

This document provides practical examples of how to use the Custom AI Agent Module in your Elsa workflows.

## Example 1: Simple Q&A Agent

**Scenario**: Create a basic question-answering agent

### Workflow Structure:
```
[Trigger] → [AgentExecutor] → [Output Response]
```

### Configuration:

**AgentExecutor Activity**:
- UserMessage: `${input.Question}`
- AgentName: "Q&A Assistant"
- SystemPrompt: "You are a knowledgeable assistant. Provide clear, concise answers."
- ModelProvider: "OpenAI"
- ModelName: "gpt-3.5-turbo"
- ApiKey: `${Variables.OpenAI_ApiKey}`
- Temperature: 0.7
- MaxTokens: 500

**Input Variables**:
- Question (string): The user's question

**Output**:
- Use the `Response` output property to get the answer

---

## Example 2: Multi-turn Conversation Agent

**Scenario**: Maintain context across multiple user interactions

### Workflow Structure:
```
[HTTP Endpoint] → [AgentExecutor] → [HTTP Response]
```

### Configuration:

**AgentExecutor Activity**:
- UserMessage: `${Request.Body.message}`
- ConversationId: `${Request.Body.conversationId}` or auto-generate
- PersistHistory: true
- MaxHistoryTurns: 20
- SystemPrompt: "You are a helpful customer service agent. Remember the context of the conversation."
- ModelProvider: "OpenAI"
- ModelName: "gpt-3.5-turbo"
- ApiKey: `${Variables.OpenAI_ApiKey}`

**API Request Format**:
```json
{
  "message": "Hello, I need help with my order",
  "conversationId": "user123-session456"
}
```

The agent will remember previous messages in the conversation!

---

## Example 3: Local Model with Ollama

**Scenario**: Use a local LLM model for privacy or offline use

### Prerequisites:
1. Install Ollama: `https://ollama.ai`
2. Pull a model: `ollama pull llama2`
3. Ensure Ollama is running: `ollama serve`

### Workflow Structure:
```
[Trigger] → [AgentExecutor] → [Process Response]
```

### Configuration:

**AgentExecutor Activity**:
- UserMessage: `${input.Prompt}`
- ModelProvider: "Ollama"
- ModelName: "llama2"
- ApiEndpoint: "http://localhost:11434/api/chat"
- SystemPrompt: "You are a helpful coding assistant specializing in C#."
- Temperature: 0.5
- MaxTokens: 1000

**Note**: No API key needed for Ollama!

---

## Example 4: Agent with Custom Tool Registration

**Scenario**: Create an agent that can use a calculator tool

### Workflow 1: Calculator Tool Workflow
Create a workflow named "Calculator" with ID: `calculator-workflow`

**Inputs**:
- operation (string): "add", "subtract", "multiply", "divide"
- a (number)
- b (number)

**Activities**:
1. Switch activity on `operation`
2. Calculate result based on operation
3. Set output variable `result`

**Outputs**:
- result (number)

### Workflow 2: Agent Workflow

**Structure**:
```
[RegisterTool] → [AgentExecutor] → [Output]
```

**RegisterTool Configuration**:
- ToolId: "calculator"
- ToolName: "Calculator"
- Description: "Performs basic arithmetic: add, subtract, multiply, divide. Requires parameters: operation (string), a (number), b (number)"
- ToolType: "Flow"
- WorkflowDefinitionId: "calculator-workflow"
- ParametersSchema: 
```json
{
  "type": "object",
  "properties": {
    "operation": {"type": "string", "enum": ["add", "subtract", "multiply", "divide"]},
    "a": {"type": "number"},
    "b": {"type": "number"}
  },
  "required": ["operation", "a", "b"]
}
```

**AgentExecutor Configuration**:
- UserMessage: "Calculate 25 times 4"
- ToolIds: "calculator"
- SystemPrompt: "You are a math assistant. Use the calculator tool for arithmetic operations."
- ModelProvider: "OpenAI"
- ModelName: "gpt-4"
- ApiKey: `${Variables.OpenAI_ApiKey}`

---

## Example 5: Multi-Agent System

**Scenario**: Route queries to specialized agents

### Workflow Structure:
```
[HTTP Endpoint] 
    → [Router Agent] 
    → [Switch]
        → [Technical Agent]
        → [Sales Agent]
        → [Support Agent]
    → [HTTP Response]
```

### Configuration:

**Router Agent**:
- UserMessage: `${Request.Body.query}`
- SystemPrompt: "Analyze the user's query and respond with ONLY one word: 'technical', 'sales', or 'support'"
- ModelProvider: "OpenAI"
- ModelName: "gpt-3.5-turbo"
- Temperature: 0.1
- MaxTokens: 10

**Switch Activity**: Branch on router response

**Technical Agent**:
- SystemPrompt: "You are a technical support expert. Help users with technical issues."
- UserMessage: `${Request.Body.query}`

**Sales Agent**:
- SystemPrompt: "You are a sales representative. Answer questions about products and pricing."
- UserMessage: `${Request.Body.query}`

**Support Agent**:
- SystemPrompt: "You are a customer support agent. Help with general inquiries and issues."
- UserMessage: `${Request.Body.query}`

---

## Example 6: Agent with Database Lookup Tool

**Scenario**: Agent that can query a database

### Workflow 1: Database Lookup Tool
Create workflow: `customer-lookup-workflow`

**Activities**:
1. ExecuteSQL activity
   - Query: `SELECT * FROM customers WHERE id = ${Input.customerId}`
2. Set output with customer data

### Workflow 2: Customer Service Agent

**Structure**:
```
[RegisterTool] → [AgentExecutor] → [Response]
```

**RegisterTool Configuration**:
- ToolId: "customer-lookup"
- ToolName: "CustomerLookup"
- Description: "Look up customer information by ID. Parameter: customerId (string)"
- ToolType: "Flow"
- WorkflowDefinitionId: "customer-lookup-workflow"

**AgentExecutor Configuration**:
- UserMessage: `${input.Query}`
- ToolIds: "customer-lookup"
- SystemPrompt: "You are a customer service agent with access to customer data via the CustomerLookup tool."

---

## Example 7: Conversation Reset

**Scenario**: Clear conversation history when starting a new topic

### Workflow Structure:
```
[HTTP Endpoint] 
    → [If: IsNewConversation]
        → [ClearConversation]
    → [AgentExecutor]
    → [HTTP Response]
```

**ClearConversation Configuration**:
- AgentId: "customer-agent"
- ConversationId: `${Request.Query.conversationId}`

**AgentExecutor Configuration**:
- ConversationId: `${Request.Query.conversationId}`
- PersistHistory: true

---

## Best Practices

### 1. API Key Management
Store API keys in Elsa variables or configuration, not hardcoded:
```csharp
ApiKey: ${Variables.OpenAI_ApiKey}
```

### 2. Temperature Settings
- **Creative tasks** (writing, brainstorming): 0.7-0.9
- **Factual tasks** (Q&A, data extraction): 0.1-0.3
- **Balanced** (general assistance): 0.5-0.7

### 3. System Prompts
Be specific about:
- Role/expertise
- Tone and style
- Response format
- Constraints or guidelines

Example:
```
You are a professional technical writer. 
Provide clear, concise explanations suitable for beginners.
Use simple language and avoid jargon.
Always include a practical example.
Keep responses under 200 words.
```

### 4. Conversation Management
- Use meaningful conversation IDs (user ID + session ID)
- Set appropriate MaxHistoryTurns based on context needs
- Clear conversations when starting new topics
- Monitor conversation history for token limits

### 5. Tool Descriptions
Make tool descriptions clear and detailed:
```
"WeatherLookup: Gets current weather for a location. 
Requires parameter 'city' (string). 
Returns temperature, conditions, and forecast."
```

### 6. Error Handling
Always handle potential failures:
```
[AgentExecutor]
    → [If: Success]
        → [Process Response]
    → [Else]
        → [Error Handler]
```

### 7. Local vs Cloud Models
- **Cloud (OpenAI)**: Better quality, faster, costs money, requires internet
- **Local (Ollama)**: Free, private, slower, works offline, lower quality
- Use local for development/testing, cloud for production

---

## Troubleshooting

### Issue: "Unable to complete the request"
- Check API key is correct
- Verify API endpoint is accessible
- Check network connectivity
- Review logs for detailed errors

### Issue: Agent doesn't use tools
- Verify tools are registered before agent execution
- Check tool IDs match exactly
- Ensure tool descriptions are clear
- Consider using GPT-4 for better tool use

### Issue: Conversation context lost
- Verify PersistHistory is true
- Check ConversationId is consistent
- Ensure MaxHistoryTurns is sufficient
- Confirm same AgentId is used

### Issue: Slow responses
- Reduce MaxTokens
- Use faster model (gpt-3.5-turbo vs gpt-4)
- Consider local models for simple tasks
- Check network latency

---

## Advanced Patterns

### Chaining Agents
```
[Input] → [Agent1: Analyzer] → [Agent2: Responder] → [Output]
```

### Agent with Validation
```
[Agent] → [Validator] → [If Valid] → [Accept] [Else] → [Regenerate]
```

### Rate-Limited Agent
```
[Check Rate Limit] → [If OK] → [Agent] → [Update Limit]
```

### Agent with Fallback
```
[Agent (GPT-4)] → [If Error] → [Agent (GPT-3.5)] → [If Error] → [Default Response]
```

---

For more information, see the main [README.md](README.md) in the CustomAgentModule directory.
