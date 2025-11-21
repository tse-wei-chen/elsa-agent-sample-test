# Run the server(dotnet 8)

If you use VS Code, press `F5` to start the server.

Or run it from PowerShell / the command line:

```powershell
dotnet run --project .\ElsaServer
```

After the server starts, open the studio in your browser:

- `https://localhost:5001` (HTTPS)

That's all — kept simple.


## Project Update Notes

This project is based on [jdevillard/Elsa-Authentication-Samples, src/01-BasicAuth](https://github.com/jdevillard/Elsa-Authentication-Samples/tree/main/src/01-BasicAuth).

### Modifications Made (Target: Full Feature)

- ✅ Upgraded Elsa packages to version 3.5.1
- ✅ Added Elsa.Agents extensions
- ⬛ WebHook extensions
- ✅ Telnyx extensions
- ✅ Workflow Contexts extensions
- ✅ Elsa Blob Storage extensions
- ✅ SQL extensions
- ✅ Http extensions
- ✅ **Custom AI Agent Module** (NEW)

### Custom AI Agent Module

A custom Elsa module has been added that provides AI agent functionality similar to n8n's agent capabilities:

**Features:**
- 🤖 Multiple AI model providers (OpenAI, Ollama for local models)
- 🔧 Tool system - create custom tools or use workflows as tools
- 💬 Conversation persistence and history management
- ⚙️ Full configuration: system prompts, temperature, token limits
- 🔒 Secure implementation with CodeQL verified

**Available Activities:**
- `Agent Executor` - Execute AI agents with model and tool configuration
- `Register Tool` - Register custom or flow-based tools for agents
- `Clear Conversation` - Manage conversation history

**Documentation:**
- 📖 [Module Documentation](ElsaServer/CustomAgentModule/README.md) - Complete API reference
- 📝 [Usage Examples](ElsaServer/CustomAgentModule/EXAMPLES.md) - 7 practical workflow examples

**Quick Start:**
1. Configure OpenAI API key in app settings or use Ollama for local models
2. Open Elsa Studio at `https://localhost:5001`
3. Look for activities under "AI Agents" category
4. See examples in EXAMPLES.md for detailed usage patterns

This module enables you to build intelligent workflows with AI agents that can use tools, maintain conversation context, and integrate with existing Elsa workflows!

Please refer to the original repository for authentication sample details.  
Changes in this fork focus on updating package dependencies and introducing new Elsa agent extensions for further development and experimentation.
