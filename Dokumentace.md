# =====================================================================
# SOUBOR:   keep_info.txt
# PROJEKT:  PortalMCXI
# VERZE:    v74.0
# ZMĚNA:    2026-05-16
# AUTOR:    Ing. Roman Fišer
# POPIS:    Dokumentace projektu. Přidán detailní popis Docker architektury.
# =====================================================================

## 1. ZÁKLADNÍ INFRASTRUKTURA A SÍTĚ
- Provozovatel (VPS): Contabo (Lokalita Hub Europe)
- IP Adresy: 62.84.181.247 / IPv6: 2a02:c207:2299:1037::1
- Operační systém: Ubuntu 24.04
- Orchestrace: Docker Compose (v3.25) v nezávislém multi-kontejnerovém režimu
- CI/CD: Azure DevOps Pipelines (5-fázové paralelní nasazení)
- Vnější reverzní proxy: Nginx Proxy Manager (NPM) běžící v síti "nginx-proxy-manager_default"


## 2. MAPOVÁNÍ CEST A NÁZVŮ (GIT VS. VPS VS. DOCKER)

### A. Kořenový adresář projektu
- Git repozitář: Kořenová složka
- Cesta na VPS: /home/roman/portal-mcxi/

### B. Frontend (React 18 / Vite / Tailwind)
- Git složka: /frontend/
- Cesta na VPS: /home/roman/portal-mcxi/frontend/
- Název služby (docker-compose): frontend
- Název kontejneru: portal_frontend

### C. Backend (.NET 10 API)
- Git složka: /backend/
- Cesta na VPS: /home/roman/portal-mcxi/backend/
- Složka projektu v Gitu: /backend/PortalMCXIBackend/
- Název služby (docker-compose): api
- Název kontejneru: portal_api

### D. Databáze a Osobní web
- DB služba: db (Kontejner: portal_db, PostgreSQL 16)
- Osobní web: personal_web (Kontejner: roman_personal)


## 3. DOCKER ARCHITEKTURA A KONFIGURACE (docker-compose.yml)
Docker zde slouží k absolutní izolaci běhového prostředí. Server Contabo díky němu nepotřebuje mít nainstalované ani .NET SDK, ani Node.js. Vše řídí soubor docker-compose.yml.

### A. Služba "api" (Backend)
- Konfigurace: Místo definice "image" a stahování zdrojových kódů přes "volumes" (což způsobovalo chybu MSB1003, protože Docker nenašel projektový soubor v hlavní složce), používá direktivu "build: context: ./backend".
- Co to dělá: Říká Compose, aby si přečetl "Dockerfile" ze složky backend. Dockerfile následně provede dvoufázové sestavení (Multi-stage build). Najde .csproj, zkompiluje ho a vytvoří malý, bezpečný obraz pouze s finální DLL knihovnou.
- Porty: Běží striktně na portu 5000 uvnitř kontejneru i navenek.

### B. Služba "frontend" (React)
- Konfigurace: Na rozdíl od API se zde frontend nekompiluje natvrdo, ale připojuje se živě. Využívá "image: node:20-alpine" a "volumes: - ./frontend:/app".
- Co to dělá: Spouští Vite jako vývojový server ("npm run dev -- --host 0.0.0.0"). Změny v kódu z Gitu se přes Pipeline okamžitě propisují na disk VPS a díky zapnutému "WATCHPACK_POLLING=true" je Vite ihned zobrazí.
- Ochrana proti pádu Vite: Direktiva "volumes: - /app/node_modules" vytváří tzv. anonymní volume. Chrání to kontejner před tím, aby si přepsal své linuxové moduly těmi, které by případně přišly z tvého lokálního prostředí na Windows.

### C. Služba "db" (PostgreSQL)
- Konfigurace: "image: postgres:16-alpine" s heslem a jménem předaným přes "environment".
- Co to dělá: Poskytuje databázi pro portál. Data by při smazání kontejneru zmizela, proto je zde mapováno perzistentní úložiště "volumes: - db_data:/var/lib/postgresql/data".

### D. Sítě (Networks)
Kontejnery jsou izolované, ale musí spolu mluvit.
- "portal_network": Vnitřní síť (bridge). Umožňuje, aby na sebe frontend, api a db viděly pouhým zavoláním svého jména (např. API se k DB připojuje přes adresu "db", nikoliv přes IP).
- "npm_network": Externí síť, do které je napojen Nginx Proxy Manager. Díky ní může NPM směrovat vnější provoz z internetu (z adres api.rosimcxi.eu) přímo do kontejneru "portal_api" bez vystavování portů ven.


## 4. NASAZOVACÍ PIPELINE (Azure DevOps)
- Fáze 1-3 (Build): Balí zdrojové soubory C#, JSX a konfigurační YAML soubory.
- Fáze 4 (Web Hub): Nahraje frontend. Promaže lokální "node_modules" a přes příkaz "docker compose rm -s -v -f frontend" odstraní zablokované volumes. Obsahuje "failOnStdErr: false", aby Pipeline nespadla na falešných varováních od Dockeru.
- Fáze 5 (API & Log): Nahraje backend. Příkazem "docker compose build --no-cache api" vynutí 100% čistou kompilaci C# projektu. Následně nasbírá data z Linuxu (RAM, Disk, top procesy) a logy (API, Nginx) do souboru "build_info.json" pro zobrazení ve webové diagnostice.