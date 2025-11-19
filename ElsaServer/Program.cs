using Elsa.Agents;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Sql.Client;
using Elsa.Sql.Extensions;
using Elsa.Sql.PostgreSql;
using Elsa.Sql.Sqlite;
using Elsa.Sql.SqlServer;
using Elsa.Tenants.Extensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

var services = builder.Services;
var configuration = builder.Configuration;

services.AddElsa(elsa =>
{
    elsa.UseIdentity(identity =>
    {
        identity.TokenOptions = options =>
        {
            options.Issuer = configuration["TokenOptions:Issuer"] ?? "http://elsa.api";
            options.Audience = configuration["TokenOptions:Audience"] ?? "http://elsa.api";
            options.AccessTokenLifetime = TimeSpan.Parse(configuration["TokenOptions:AccessTokenLifetime"] ?? "12:00:00");
            options.SigningKey = configuration["TokenOptions:SigningKey"] ?? "please-change-this-signing-key";
        };
        identity.UseAdminUserProvider();
    });
    elsa.UseDefaultAuthentication();
    elsa.UseTenants();
    elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite("Data Source=elsa.sqlite.db")));
    elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseSqlite("Data Source=elsa.sqlite.db")));
    elsa.UseScheduling();
    elsa.UseJavaScript();
    elsa.UseLiquid();
    elsa.UseCSharp();
    elsa.UseWorkflowsApi();
    elsa.AddActivitiesFrom<Program>();
    elsa.AddWorkflowsFrom<Program>();
    // Agent module configuration
    elsa.UseAgents();
    elsa.UseAgentActivities();
    elsa.UseAgentsApi();
    elsa.UseAgentPersistence(persistence => persistence.UseEntityFrameworkCore(ef => ef.UseSqlite("Data Source=elsa-agents.sqlite.db")));
    // Telnyx module configuration
    elsa.UseTelnyx();
    // WorkflowContext configuration
    elsa.UseWorkflowContexts();
    // FileStorage configuration
    elsa.UseFileStorage();
    // SQL configuration
    elsa.UseSql(options =>
    {
        options.Clients = client =>
        {
            client.Register<ISqlClient>("MySql");
            client.Register<PostgreSqlClient>("PostgreSql");
            client.Register<SqliteClient>("Sqlite");
            client.Register<SqlServerClient>("Sql Server");
        };
    });
    // HTTP configuration
    elsa.UseHttp(http => http.ConfigureHttpOptions = options => configuration.GetSection("Http").Bind(options));
});

services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*")));
services.AddRazorPages(options => options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));
services.AddControllers();
services.AddEndpointsApiExplorer();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseRouting();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.MapFallbackToPage("/_Host");
app.Run();