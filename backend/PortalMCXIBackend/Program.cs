// =====================================================================
// Soubor: Program.cs
// Projekt: PortalMCXIBackend
// Verze: v12.0 (Rozdělená architektura)
// Autor: Ing. Roman Fišer
// Popis: Z tohoto souboru se stala čistá centrála. Pouze nastavuje
//        prostředí (DI, Middleware, CORS) a pak zavolá jednotlivé
//        moduly z adresáře Endpoints.
// =====================================================================

using Scalar.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using PortalMCXIBackend.Models;
using PortalMCXIBackend.Endpoints; // Načtení našich nových souborů!

var builder = WebApplication.CreateSlimBuilder(args);

// ---------------------------------------------------------------------
// 1. REGISTRACE SLUŽEB A CORS
// ---------------------------------------------------------------------
builder.Services.AddCors(options => {
options.AddPolicy("PortalPolicy", policy => {
policy.WithOrigins("https://rosimcxi.eu", "https://roman.rosimcxi.eu", "https://api.rosimcxi.eu", "http://localhost:5173")
.AllowAnyMethod()
.AllowAnyHeader();
});
});

// Zajištění kompatibility s Nginx Proxy Managerem. API musí vědět,
// že je schované za proxy a přijímat reálné IP adresy.
builder.Services.Configure(options =>
{
options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
options.KnownNetworks.Clear();
options.KnownProxies.Clear();
});

// Nástroje pro dokumentaci API
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Zapnutí Native AOT kompilátoru JSON (používá třídu z AppModels.cs)
builder.Services.ConfigureHttpJsonOptions(options =>
{
options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

// ---------------------------------------------------------------------
// 2. MIDDLEWARE PIPELINE (Pořadí je zde extrémně důležité!)
// ---------------------------------------------------------------------
app.UseForwardedHeaders(); // Zpracuj hlavičky od Nginx
app.UseCors("PortalPolicy"); // Aplikuj bezpečnostní filtry

// Vygeneruje API dokumentaci ve formátu JSON
app.MapOpenApi();

// Vygeneruje krásný tmavý prohlížeč dokumentace (Scalar)
app.MapScalarApiReference(options =>
{
options.Title = "PortalMCXI API Reference";
options.Theme = ScalarTheme.Moon;
});

// ---------------------------------------------------------------------
// 3. MAPOVÁNÍ MODULŮ (Rozšiřující metody)
// Zde připojíme všechny naše rozdělené soubory k hlavní aplikaci.
// ---------------------------------------------------------------------
app.MapSystemEndpoints();
app.MapEsoterikaEndpoints();
app.MapCasNaEndpoints();

// ---------------------------------------------------------------------
// 4. SPUŠTĚNÍ SERVERU
// ---------------------------------------------------------------------
app.Run();