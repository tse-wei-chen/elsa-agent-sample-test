using Elsa.Agents;
using Elsa.Api.Client.Extensions;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Tenants.Extensions;
using Microsoft.AspNetCore.Mvc;

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
        //AGENT MODULE CONFIGURATION
        .UseAgents()
        .UseAgentActivities()
        .UseAgentsApi()
        .UseAgentPersistence(persistence => persistence.UseEntityFrameworkCore(ef => ef.UseSqlite("Data Source=elsa-agents.sqlite.db")))
        //WEBHOOKS MODULE CONFIGURATION
        .UseWebhooks(webhooks => webhooks.ConfigureSinks += options => builder.Configuration.GetSection(key: "Webhooks").Bind(options))
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