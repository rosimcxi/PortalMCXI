# =====================================================================
# SOUBOR:   Dockerfile
# PROJEKT:  PortalMCXI
# VERZE:    v5.1 (Fix cesty k PortalMCXIBackend/PortalMCXIBackend.csproj)
# AUTOR:    Ing. Roman Fišer
# POPIS:    Profesionální multi-stage build. Opravena cesta k projektu,
#           který je v podsložce podle struktury repozitáře.
# =====================================================================

# --- 1. FÁZE: SESTAVENÍ (BUILD) ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Kopírujeme soubor projektu z podsložky do stejné struktury v Dockeru
# To je nutné pro správné fungování relativních cest v .csproj
COPY ["PortalMCXIBackend/PortalMCXIBackend.csproj", "PortalMCXIBackend/"]
RUN dotnet restore "PortalMCXIBackend/PortalMCXIBackend.csproj"

# Kopírujeme zbytek zdrojových kódů (celou složku backend)
COPY . .

# Přepneme se do složky s projektem pro samotné sestavení
WORKDIR "/src/PortalMCXIBackend"

# Sestavení aplikace v Release režimu
RUN dotnet publish "PortalMCXIBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- 2. FÁZE: BĚH (RUNTIME) ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Kopírujeme pouze hotové binárky z první fáze
COPY --from=build /app/publish .

# Nastavení portů a prostředí
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Spouštíme přímo zkompilovanou knihovnu
ENTRYPOINT ["dotnet", "PortalMCXIBackend.dll"]