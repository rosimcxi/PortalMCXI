using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using PortalMCXIBackend.Endpoints;
using PortalMCXIBackend.Models;
using Scalar.AspNetCore;
using PortalMCXIBackend.Services;

var builder = WebApplication.CreateBuilder(args);
var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy("PortalPolicy", policy =>
{
    if (origins.Length > 0) policy.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Proxy z Docker sítě přidat explicitně konfigurací; nedůvěřovat libovolnému klientovi.
    foreach (var proxy in builder.Configuration.GetSection("Proxy:KnownProxies").Get<string[]>() ?? [])
        options.KnownProxies.Add(IPAddress.Parse(proxy));
});
builder.Services.AddSingleton<PersonalCalculationService>();
builder.Services.AddHttpClient<MorningInfoService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(8);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("PortalMCXI/1.0");
});
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default));

var app = builder.Build();
app.UseForwardedHeaders();
app.UseCors("PortalPolicy");
app.MapGet("/api/health", () => Results.Ok(new HealthStatus("OK"))).WithName("Health");
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "PortalMCXI API Reference";
    options.Theme = ScalarTheme.Moon;
});
app.MapSystemEndpoints();
app.MapEsoterikaEndpoints();
app.MapInfoEndpoints();

// Moduly zatím používají demo data bez persistence a autentizace.
if (app.Environment.IsDevelopment() && builder.Configuration.GetValue("Portal:EnableDemoModules", true))
{
    app.MapCasNaEndpoints();
    app.MapMonitoringEndpoints();
    app.MapSpravaProjektuEndpoints();
    app.MapUkolnicekEndpoints();
    app.MapSnippetsEndpoints();
    app.MapUzivateleEndpoints();
}
app.Run();
