using Elsa.Agents;
using Elsa.Api.Client.Extensions;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Sql.Client;
using Elsa.Sql.Extensions;
using Elsa.Sql.PostgreSql;
using Elsa.Sql.Sqlite;
using Elsa.Sql.SqlServer;
using Elsa.Studio.Webhooks.Extensions;
using Elsa.Tenants.Extensions;
using Microsoft.AspNetCore.Mvc;
using WebhooksCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

var services = builder.Services;
var configuration = builder.Configuration;

services
    .AddElsa(elsa => elsa
        .UseIdentity(identity =>
        {
            identity.TokenOptions = options =>
            {
                options.Issuer = configuration["TokenOptions:Issuer"] ?? "http://elsa.api";
                options.Audience = configuration["TokenOptions:Audience"] ?? "http://elsa.api";
                options.AccessTokenLifetime = TimeSpan.Parse(configuration["TokenOptions:AccessTokenLifetime"] ?? "12:00:00");
                options.SigningKey = configuration["TokenOptions:SigningKey"] ?? "please-change-this-signing-key";
            };
            identity.UseAdminUserProvider();
        })
        .UseDefaultAuthentication()
        .UseTenants()
        .UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite("Data Source=elsa.sqlite.db")))
        .UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseSqlite("Data Source=elsa.sqlite.db")))
        .UseScheduling()
        .UseJavaScript()
        .UseLiquid()
        .UseCSharp()
        .UseWorkflowsApi()
        .UseRealTimeWorkflows()
        .AddActivitiesFrom<Program>()
        .AddWorkflowsFrom<Program>()
        // Agent module configuration
        .UseAgents()
        .UseAgentActivities()
        .UseAgentsApi()
        .UseAgentPersistence(persistence => persistence.UseEntityFrameworkCore(ef => ef.UseSqlite("Data Source=elsa-agents.sqlite.db")))
        // Webhooks module configuration
        .UseWebhooks(webhooks =>
        {
            webhooks.ConfigureSinks += options =>
            builder.Configuration.GetSection("Webhooks")
            .Bind(options);
        })
        // Telnyx module configuration
        .UseTelnyx()
        .UseWorkflowContexts()
        .UseFileStorage()
        .UseSql(options =>
            {
                options.Clients = client =>
                {
                    client.Register<ISqlClient>("MySql");
                    client.Register<PostgreSqlClient>("PostgreSql");
                    client.Register<SqliteClient>("Sqlite");
                    client.Register<SqlServerClient>("Sql Server");
                };
            })
    );

services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*")));
services.AddRazorPages(options => options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));

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
app.UseWorkflowsSignalRHubs();

app.MapFallbackToPage("/_Host");
app.Run();