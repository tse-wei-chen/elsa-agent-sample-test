using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Studio.Login.Extensions;
using Microsoft.AspNetCore.Identity;
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
        .UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()))
        .UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseSqlite()))
        .UseScheduling()
        .UseJavaScript()
        .UseLiquid()
        .UseCSharp()
        .UseWorkflowsApi()
        .UseRealTimeWorkflows()
        .AddActivitiesFrom<Program>()
        .AddWorkflowsFrom<Program>()
        .UseAgents()
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
// Temporary fallback for missing AI API endpoints used by the Studio client.
// Returns an empty list response for API keys so the Blazor client receives JSON
// instead of the SPA HTML page when the AI module is not installed.
var aiGroup = app.MapGroup("/elsa/api/ai");
aiGroup.MapMethods("/{**endpoint}", ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD"], (string endpoint) =>
    Results.Json(new { path = endpoint, items = Array.Empty<object>(), total = 0 })
);

app.MapFallbackToPage("/_Host");
app.Run();