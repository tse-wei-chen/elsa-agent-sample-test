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

Please refer to the original repository for authentication sample details.  
Changes in this fork focus on updating package dependencies and introducing new Elsa agent extensions for further development and experimentation.
