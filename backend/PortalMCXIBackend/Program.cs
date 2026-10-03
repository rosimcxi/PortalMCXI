using Microsoft.AspNetCore.HttpOverrides;
using PortalMCXIBackend.Endpoints;
using PortalMCXIBackend.Models;
using PortalMCXIBackend.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PortalPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://rosimcxi.eu",
                "https://roman.rosimcxi.eu",
                "https://api.rosimcxi.eu",
                "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<MorningInfoService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(8);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("PortalMCXI/1.0");
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(
        0,
        AppJsonSerializerContext.Default);
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseCors("PortalPolicy");

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "PortalMCXI API Reference";
    options.Theme = ScalarTheme.Moon;
});

app.MapSystemEndpoints();
app.MapEsoterikaEndpoints();
app.MapCasNaEndpoints();
app.MapInfoEndpoints();

app.Run();
