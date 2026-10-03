# PortalMCXI

Repozitář projektu: https://github.com/rosimcxi/PortalMCXI

Import ze souboru `PortalMCXI (1).zip` dne 3. 10. 2026. Popis přenosu a bezpečnostních úprav: [docs/IMPORT.md](docs/IMPORT.md).

Aktuální projektový rozcestník: [řídicí dokument](docs/PROJECT_CONTROL.md), [revize architektury](docs/ARCHITECTURE_REVIEW.md), [propojení s RFU](docs/RFU_INTEGRATION.md), [další postup](docs/NEXT_STEPS.md), [instrukce pro vývoj](AGENTS.md) a [převzatá dokumentace RFU](docs/rfu/README.md).

Portal smí využívat nejvýše **30 % zdrojů RFU**. [Politika a stav vynucení](docs/RFU_RESOURCE_POLICY.md). Dokud vynucení není ověřeno, RFU Portal práce se nespouští.

Níže je původní dokumentace. Popis Azure DevOps, systemd, cest a nasazení je historický podklad; skutečný stav Contabo zatím nebyl ověřen. Import neprovádí nasazení ani nepotvrzuje funkčnost aplikace.

---

Kompletní manuál: Architektura a automatické nasazení PortalMCXI
Tento dokument slouží jako hlavní technická reference pro projekt PortalMCXI. Popisuje rozložení celého systému, propojení repozitáře se serverem a kompletní návod na konfiguraci CI/CD (Continuous Integration / Continuous Deployment) přes Azure DevOps.
1. SOUHRN INFRASTRUKTURY A ARCHITEKTURY
Před samotným nastavením je klíčové znát, kde se jaká část systému nachází a jak spolu komunikují.
A) Git Repozitář (Azure DevOps)
Zdrojové kódy jsou verzovány v Azure DevOps a strukturovány takto:
/backend/PortalMCXIBackend/ - C# .NET 10 řešení využívající Clean Architecture (Domain, Application, Infrastructure, Api). Spouštěcí projekt končí na *Api.csproj.
/frontend/ - React frontend postavený na Vite a Tailwind CSS v4. Kód se kompiluje do složky dist.
/azure-pipelines.yml - Konfigurační soubor definující kroky automatického nasazení.
B) Cílový Server (Produkce)
Poskytovatel: Contabo VPS (Cloud VPS 20 NVMe)
OS: Ubuntu 24.04
IP adresa: 62.84.181.247
Zajištění přístupu: Nginx Proxy Manager (NPM) - stará se o SSL certifikáty a směrování domén na interní porty.
C) Rozložení složek na VPS
Agent Azure DevOps: /home/roman/portal-mcxi/myagent/
Zde žije program, který naslouchá Azure DevOps, stahuje zdrojáky a kompiluje je. Tuto složku nikdy nemaž.
Produkční Frontend (Live): /var/www/portalmcxi/frontend/
Zde Nginx hledá HTML a JS soubory tvého Reactu. Složka se při každém nasazení automaticky promaže a nahradí novou verzí.
Produkční Backend API (Live): /var/www/portalmcxi/api/
Zde leží zkompilované .dll soubory C# aplikace. Běží jako služba na pozadí.
D) Systémové služby
Služba Backendu: portalmcxi.service (Systemd)
Stará se o to, aby tvé .NET 10 API běželo nepřetržitě a automaticky se zapnulo po restartu serveru.
Služba Agenta: vsts.agent.ROSI-MCXI.Contabo.VPS-Contabo.service
Udržuje naživu spojení s Azure DevOps.
2. PŘÍPRAVA V AZURE DEVOPS
Než nasadíme agenta, musíme mu v cloudu připravit přístupová práva.
A) Vytvoření Agent Poolu
V Azure DevOps přejdi do Project settings (ozubené kolo vlevo dole).
Vyber Agent pools -> Add pool.
Pool type nastav na Self-hosted, Name na Contabo.
B) Vytvoření přístupového tokenu (PAT)
Klikni na ikonku uživatele (vpravo nahoře) -> Personal access tokens.
Klikni na New Token, pojmenuj ho (např. ContaboVPS).
V sekci Scopes zaškrtni Agent Pools (Read & manage).
Token vygeneruj a bezpečně si ho zkopíruj. Bude potřeba při instalaci agenta.
C) Vytvoření SSH připojení (Service Connection)
Toto připojení pipeline využívá k finálnímu rozkopírování artefaktů na VPS.
V Project settings přejdi do Service connections -> New service connection.
Vyber SSH.
Vyplň údaje:
Host: 62.84.181.247
Port: 22
User name: root
Password/Key: Tvé root heslo k VPS.
Service connection name: ContaboVPS
3. INSTALACE AGENTA NA CONTABO VPS
Připoj se na svůj server přes SSH (ssh root@62.84.181.247) a zadej tyto příkazy:
Stažení a rozbalení
# Vytvoření složky pro agenta
mkdir -p /home/roman/portal-mcxi/myagent
cd /home/roman/portal-mcxi/myagent

# Stažení instalačního balíčku (verzi případně zkontroluj v Azure DevOps)
wget [https://vstsagentpackage.azureedge.net/agent/3.236.1/vsts-agent-linux-x64-3.236.1.tar.gz](https://vstsagentpackage.azureedge.net/agent/3.236.1/vsts-agent-linux-x64-3.236.1.tar.gz)

# Rozbalení archivu
tar zxvf vsts-agent-linux-x64-3.236.1.tar.gz


Konfigurace a spuštění
Vzhledem k tomu, že používáš uživatele root, je nutné to před konfigurací povolit:
export AGENT_ALLOW_RUNASROOT="1"
./config.sh


Odpovědi pro průvodce konfigurací:
Server URL: https://dev.azure.com/ROSI-MCXI (nebo dle tvého přesného názvu organizace)
Authentication type: [Stiskni Enter] (zůstane výchozí PAT)
Personal access token: [Vlož zkopírovaný token z kroku 2B]
Agent pool: Contabo
Agent name: VPS-Contabo
Work folder: [Stiskni Enter] (nechá výchozí _work)
Nastavení agenta jako systémové služby
Aby agent běžel i po odhlášení z terminálu a spouštěl se po restartu serveru:
sudo ./svc.sh install
sudo ./svc.sh start
sudo ./svc.sh status # Pro ověření, že běží (musí ukazovat "active (running)")


4. KONFIGURACE PIPELINE (azure-pipelines.yml)
Tento kód vlož do souboru azure-pipelines.yml v kořenovém adresáři tvého projektu. Jde o nejnovější odladěnou verzi (ver.6).
# Azure Pipeline pro automatické nasazení projektu PortalMCXI
# Verze: ver.6
# Datum: 2026-03-25
# Architektura: .NET 10 (Backend API - Clean Architecture) + React Vite (Frontend)
# Cílový server: Contabo VPS (Ubuntu 24.04, IP: 62.84.181.247)
# Proxy: Nginx Proxy Manager (změny se propisují automaticky, není nutný reload)

trigger:
- main

pool:
  name: 'Contabo'

variables:
  buildConfiguration: 'Release'
  dotNetVersion: '10.x'
  nodeVersion: '20.x'
  sshConnection: 'ContaboVPS' 
  frontendFolder: 'frontend' 

stages:
# ==========================================
# STAGE 1: Sestavení Backendu (.NET 10 API)
# ==========================================
- stage: BuildBackend
  displayName: 'Sestavení C# API'
  jobs:
  - job: Build
    steps:
    - task: UseDotNet@2
      displayName: 'Instalace .NET 10 SDK'
      inputs:
        version: $(dotNetVersion)
        
    - task: DotNetCoreCLI@2
      displayName: 'Publikace API'
      inputs:
        command: 'publish'
        publishWebProjects: true # Inteligentně najde projekt s Web SDK a vynechá knihovny
        arguments: '-c $(buildConfiguration) -o $(Build.ArtifactStagingDirectory)/backend'
        zipAfterPublish: false
        modifyOutputPath: false
      
    - task: PublishBuildArtifacts@1
      displayName: 'Uložení API artefaktů'
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)/backend'
        ArtifactName: 'backend-dist'

# ==========================================
# STAGE 2: Sestavení Frontendu (React Vite)
# ==========================================
- stage: BuildFrontend
  displayName: 'Sestavení React Aplikace'
  jobs:
  - job: Build
    steps:
    - task: NodeTool@0
      displayName: 'Instalace Node.js'
      inputs:
        versionSpec: $(nodeVersion)
        
    - script: |
        echo "Přesouvám se do složky s frontendem: $(frontendFolder)"
        cd $(frontendFolder)
        npm install
        npm run build
      displayName: 'Instalace NPM balíčků a Build frontendu'
      
    - task: PublishBuildArtifacts@1
      displayName: 'Uložení React artefaktů'
      inputs:
        PathtoPublish: '$(frontendFolder)/dist' 
        ArtifactName: 'frontend-dist'

# ==========================================
# STAGE 3: Nasazení na Contabo VPS
# ==========================================
- stage: Deploy
  displayName: 'Nasazení na server'
  dependsOn: [BuildBackend, BuildFrontend]
  condition: succeeded()
  jobs:
  - job: Deploy
    steps:
    # 1. Stažení připravených souborů (artefaktů)
    - task: DownloadBuildArtifacts@0
      displayName: 'Stažení API artefaktů'
      inputs:
        buildType: 'current'
        downloadType: 'single'
        artifactName: 'backend-dist'
        downloadPath: '$(System.ArtifactsDirectory)'
        
    - task: DownloadBuildArtifacts@0
      displayName: 'Stažení React artefaktů'
      inputs:
        buildType: 'current'
        downloadType: 'single'
        artifactName: 'frontend-dist'
        downloadPath: '$(System.ArtifactsDirectory)'

    # 2. Kopírování API na VPS do produkční složky
    - task: CopyFilesOverSSH@0
      displayName: 'Kopírování API přes SSH'
      inputs:
        sshEndpoint: $(sshConnection)
        sourceFolder: '$(System.ArtifactsDirectory)/backend-dist'
        contents: '**'
        targetFolder: '/var/www/portalmcxi/api'
        cleanTargetFolder: false # Nechceme smazat konfigurační soubory na produkci

    # 3. Kopírování Reactu na VPS do produkční složky
    - task: CopyFilesOverSSH@0
      displayName: 'Kopírování React Frontendu přes SSH'
      inputs:
        sshEndpoint: $(sshConnection)
        sourceFolder: '$(System.ArtifactsDirectory)/frontend-dist'
        contents: '**'
        targetFolder: '/var/www/portalmcxi/frontend'
        cleanTargetFolder: true # Promaže starý HTML/JS před nahráním nového

    # 4. Restart .NET Služby
    - task: SSH@0
      displayName: 'Restart C# API služby'
      inputs:
        sshEndpoint: $(sshConnection)
        runOptions: 'inline'
        inline: |
          echo "Restartuji .NET API službu (portalmcxi.service)..."
          sudo systemctl restart portalmcxi.service
          echo "Nasazení dokončeno! Změny jsou online."


5. BĚŽNÝ PRACOVNÍ POSTUP (WORKFLOW)
Od této chvíle na Contabo server už nechodíš. Tvá práce probíhá výhradně z PC:
Vývoj: Otevřeš Visual Studio 2026 pro C# a VS Code pro Vite React. Naprogramuješ novou funkci (např. připojení DB, nový Dashboard).
Lokální test: Ověříš si u sebe na PC, že vše funguje (Swagger, npm run dev).
Commit & Push: V Gitu (např. přes Visual Studio) provedeš Commit All and Push.
Automatizace: * Azure DevOps uvidí nové soubory.
Probudí agenta myagent na tvém VPS.
Agent stáhne kód, zkompiluje C# DLL a React DIST (vytvoří artefakty).
Zkopíruje je do /var/www/portalmcxi/api a /var/www/portalmcxi/frontend.
Restartuje backend službu.
Hotovo: Do 2 minut je tvůj kód živě online a dostupný návštěvníkům webu.
6. ŘEŠENÍ PROBLÉMŮ (TROUBLESHOOTING)
Pipeline padá s chybou "Project file not found": Zkontroluj, zda jsi v Azure Pipelines YAML správně nastavil publishWebProjects: true nebo zda tvůj projekt neobsahuje překlepy v názvech adresářů (Linux je citlivý na malá/velká písmena).
Pipeline spadne na chybě cd frontend:
Ujisti se, že proměnná frontendFolder odpovídá přesně názvu složky s Reactem v Gitu (včetně velikosti písmen).
Agent je offline:
Připoj se na VPS a spusť: sudo systemctl status vsts.agent.ROSI-MCXI.Contabo.VPS-Contabo.service. Pokud neběží, restartuj ho: sudo systemctl restart vsts.agent.*
Agent hlásí chybu VS30063 (You are not authorized):
Vypršel ti platný PAT token. Vygeneruj nový v Azure DevOps, odinstaluj službu (sudo ./svc.sh uninstall), odeber konfiguraci (./config.sh remove) a proces konfigurace opakuj s novým tokenem.

