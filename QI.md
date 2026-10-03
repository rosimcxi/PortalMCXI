🚀 Rychlý tahák: Contabo VPS, Docker a OdkazyTento dokument slouží jako rychlý rozcestník a záchranná brzda pro server Contabo a projekt PortalMCXI.1. Rychlé připojení (SSH přes PowerShell)Když potřebuješ na server z tvého HP ProBooku, stačí otevřít PowerShell a zadat:ssh root@62.84.181.247(Případně ssh roman@62.84.181.247. Heslo: [REMOVED_SECRET])Cesta ke složce s projektem na serveru:cd /home/roman/portal-mcxi2. Důležité webové administraceZde jsou odkazy na správu tvé infrastruktury. Vše běží na tvé Contabo IP adrese 62.84.181.247.Nginx Proxy Manager (Směrování a SSL):http://62.84.181.247:81Portainer (Vizuální správa Dockeru):https://62.84.181.247:9443 (Případně port 9000 pro čisté HTTP)API Dokumentace (Scalar v .NET 10):https://api.rosimcxi.eu/scalar/v1Hlavní Diagnostický Dashboard:https://rosimcxi.eu3. Záchranný skript (Oprava sítě a NPM)Pokud se po updatu zblázní sítě v Dockeru (chyba 502, mrtvý Nginx), připoj se přes SSH, zkopíruj tento blok a rovnou ho odmáčkni v terminálu. Skript vše čistě restartuje a znovu vytvoří propojení sítí:cd /home/roman/portal-mcxi

echo "1. Zastavuji projekt..."
docker compose down

echo "2. Zastavuji Nginx Proxy Manager..."
docker stop nginx-proxy-manager-app-1

echo "3. Znovu vytvarim smazanou sit NPM..."
docker network create nginx-proxy-manager_default || true

echo "4. Startuji Nginx a pripojuji ho do site..."
docker start nginx-proxy-manager-app-1
docker network connect nginx-proxy-manager_default nginx-proxy-manager-app-1 || true

echo "5. Startuji cely PortalMCXI..."
docker compose up -d

echo "✅ OPRAVA DOKONCENA! Jdi do NPM a uloz certifikaty."
4. Běžné Docker příkazyKdyž jsi v terminálu, tyhle příkazy se vždycky hodí:docker ps - Zobrazí seznam běžících kontejnerů a jejich stav.docker logs -f portal_api - Živé sledování logů z .NET API (zavřeš pomocí Ctrl+C).docker logs -f nginx-proxy-manager-app-1 - Živé sledování toho, co Nginx dělá.docker compose build --no-cache api - Tvrdá rekompilace C# kódu, pokud Pipeline z nějakého důvodu selže.