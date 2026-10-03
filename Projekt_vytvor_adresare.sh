#---FILENAME: install_portal.sh---
#!/bin/bash

# ==============================================================================

# PROJEKT: PortalMCXI (Standard 2026)

# AUTOR: Senior Developer (.NET 10 & React 19)

# ÚČEL: Kompletní inicializace repozitáře a zdrojových kódů

# ==============================================================================

PROJECT_NAME="PortalMCXI"
ROOT_DIR=$(pwd)/$PROJECT_NAME

echo "?? Zahajuji instalaci projektu $PROJECT_NAME do $ROOT_DIR..."

# 1. Vytvoření základní struktury složek

mkdir -p $ROOT_DIR/backend/src
mkdir -p $ROOT_DIR/backend/tests
mkdir -p $ROOT_DIR/frontend
mkdir -p $ROOT_DIR/scripts/deployment
mkdir -p $ROOT_DIR/.github/workflows

cd $ROOT_DIR/backend

# 2. Vytvoření .NET 10 Solution a Projektů

echo "?? Generuji .NET 10 Solution a vrstvy architektury..."
dotnet new sln -n $PROJECT_NAME

# Domain (Entity, Rozhraní)

dotnet new classlib -n $PROJECT_NAME.Domain -o src/$PROJECT_NAME.Domain

# Application (Business Logika, DTOs, Mappery)

dotnet new classlib -n $PROJECT_NAME.Application -o src/$PROJECT_NAME.Application

# Infrastructure (EF Core, Npgsql, Repozitáře)

dotnet new classlib -n $PROJECT_NAME.Infrastructure -o src/$PROJECT_NAME.Infrastructure

# Web API (Controllers)

dotnet new webapi -n $PROJECT_NAME.Api -o src/$PROJECT_NAME.Api

# Unit Tests (xUnit)

dotnet new xunit -n $PROJECT_NAME.Tests.Unit -o tests/$PROJECT_NAME.Tests.Unit

# 3. Nastavení referencí (Service-Repository Pattern)

echo "?? Propojuji projekty..."
dotnet add src/$PROJECT_NAME.Application/$PROJECT_NAME.Application.csproj reference src/$PROJECT_NAME.Domain/$PROJECT_NAME.Domain.csproj
dotnet add src/$PROJECT_NAME.Infrastructure/$PROJECT_NAME.Infrastructure.csproj reference src/$PROJECT_NAME.Domain/$PROJECT_NAME.Domain.csproj
dotnet add src/$PROJECT_NAME.Api/$PROJECT_NAME.Api.csproj reference src/$PROJECT_NAME.Application/$PROJECT_NAME.Application.csproj
dotnet add src/$PROJECT_NAME.Api/$PROJECT_NAME.Api.csproj reference src/$PROJECT_NAME.Infrastructure/$PROJECT_NAME.Infrastructure.csproj
dotnet add tests/$PROJECT_NAME.Tests.Unit/$PROJECT_NAME.Tests.Unit.csproj reference src/$PROJECT_NAME.Application/$PROJECT_NAME.Application.csproj

# Přidání do solution

dotnet sln add src/$PROJECT_NAME.Domain/$PROJECT_NAME.Domain.csproj
dotnet sln add src/$PROJECT_NAME.Application/$PROJECT_NAME.Application.csproj
dotnet sln add src/$PROJECT_NAME.Infrastructure/$PROJECT_NAME.Infrastructure.csproj
dotnet sln add src/$PROJECT_NAME.Api/$PROJECT_NAME.Api.csproj
dotnet sln add tests/$PROJECT_NAME.Tests.Unit/$PROJECT_NAME.Tests.Unit.csproj

# 4. Generování základních souborů (Domain & Infrastructure)

echo "?? Generuji základní kódové soubory..."

# BaseEntity.cs

cat <<EOF > src/$PROJECT_NAME.Domain/Entities/BaseEntity.cs
namespace $PROJECT_NAME.Domain.Entities;

/// <summary>
/// Základní entita pro PostgreSQL (snake_case konvence).
/// </summary>
public abstract class BaseEntity
{
public long id { get; set; }
public DateTime created_at { get; set; } = DateTime.UtcNow;
public DateTime? updated_at { get; set; }
}
EOF

# DbContext.cs

cat <<EOF > src/$PROJECT_NAME.Infrastructure/Data/PortalDbContext.cs
using Microsoft.EntityFrameworkCore;
using $PROJECT_NAME.Domain.Entities;

namespace $PROJECT_NAME.Infrastructure.Data;

public class PortalDbContext : DbContext
{
public PortalDbContext(DbContextOptions<PortalDbContext> options) : base(options) { }

```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    // Automatický snake_case pro PostgreSQL
    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
        entity.SetTableName(entity.GetTableName()?.ToLower().Replace("entity", ""));
        foreach (var property in entity.GetProperties())
            property.SetColumnName(property.Name.ToLower());
    }
}

```

}
EOF

# 5. Inicializace Frontendu (React 19 + Vite)

echo "?? Inicializuji React 19 + TypeScript..."
cd $ROOT_DIR/frontend

# Inicializace Vite (vyžaduje Node.js)

npm create vite@latest . -- --template react-ts

# Vytvoření adresářů pro čistou React architekturu

mkdir -p src/services/api src/types/dtos src/features/dashboard src/components/layout src/hooks src/store

# 6. Finální Git a README

cd $ROOT_DIR
echo "# PortalMCXI
Moderní webová platforma postavená na .NET 10 a React 19.

## Struktura

* `backend/`: ASP.NET Core Web API (Service-Repository)
* `frontend/`: React (Vite, Bootstrap 5)
* `scripts/`: Deployment skripty pro Ubuntu 24.04 (62.84.181.247)
" > README.md

echo "bin/
obj/
node_modules/
dist/
.vs/
.vscode/
*.user
appsettings.Development.json" > .gitignore

echo "? HOTOVO! Projekt PortalMCXI je připraven v: $ROOT_DIR"
echo "?? Otevři backend/$PROJECT_NAME.sln ve Visual Studiu 2026."

---FILENAME: backend/src/PortalMCXI.Api/Program.cs---
using Microsoft.EntityFrameworkCore;
using PortalMCXI.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Nastavení PostgreSQL (Npgsql)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PortalDbContext>(options =>
options.UseNpgsql(connectionString));

// CORS pro tvoje domény
builder.Services.AddCors(options =>
{
options.AddPolicy("PortalPolicy", policy =>
{
policy.WithOrigins("[https://commendation.eu](https://www.google.com/search?q=https://commendation.eu)", "[https://rosimcxi.eu](https://www.google.com/search?q=https://rosimcxi.eu)", "[https://wisdomes.eu](https://www.google.com/search?q=https://wisdomes.eu)")
.AllowAnyMethod()
.AllowAnyHeader();
});
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PortalPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();

---FILENAME: .github/workflows/deploy.yml---
name: Deploy PortalMCXI to Ubuntu VPS

on:
push:
branches: [ main ]

jobs:
deploy:
runs-on: ubuntu-latest
steps:
- uses: actions/checkout@v4

```
  - name: Setup .NET 10
    uses: actions/setup-dotnet@v4
    with:
      dotnet-version: '10.0.x'

  - name: Build and Publish Backend
    run: |
      dotnet publish backend/src/PortalMCXI.Api/PortalMCXI.Api.csproj -c Release -o ./publish
      
  # Zde by následoval SCP přenos na tvou IP 62.84.181.247 a restart systemd služby

```

---

### Jak to nyní zprovoznit na tvém Asusu:

