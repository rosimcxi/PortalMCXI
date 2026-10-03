#!/bin/bash
# Soubor: /root/install_portal.sh
# Autor: Ing. Roman Fišer
# Účel: Čistá prvotní instalace a konfigurace prostředí pro PortalMCXI
# Použití: Sudo oprávnění. Spustit pouze při zakládání serveru nebo obnově.

echo "================================================================"
echo "🚀 INSTALACE PROSTŘEDÍ PORTALMCXI (Ubuntu / .NET 10 / Nginx)"
echo "================================================================"

# 1. VYTVOŘENÍ ADRESÁŘOVÉ STRUKTURY
echo "📂 1/4 Vytvářím přesnou adresářovou strukturu..."
sudo mkdir -p /var/www/portalmcxi/frontend
sudo mkdir -p /var/www/portalmcxi/api
sudo chown -R $USER:$USER /var/www/portalmcxi
sudo chmod -R 755 /var/www/portalmcxi

# 2. VYTVOŘENÍ SYSTEMD SLUŽBY PRO .NET 10 API
echo "⚙️  2/4 Generuji službu systemd (portalmcxi.service)..."
SERVICE_FILE="/etc/systemd/system/portalmcxi.service"
DOTNET_PATH=$(which dotnet || echo "/usr/bin/dotnet")

sudo bash -c "cat > $SERVICE_FILE" <<EOF
[Unit]
Description=PortalMCXI .NET 10 Web API (Roman Fiser)
After=network.target

[Service]
WorkingDirectory=/var/www/portalmcxi/api
# Zajišťuje spuštění sestavené DLL knihovny z Azure
ExecStart=$DOTNET_PATH /var/www/portalmcxi/api/PortalMCXIBackend.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=portalmcxi
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
EOF

# 3. ZÁKLADNÍ KONFIGURACE NGINX (Lokální Proxy)
# Poznámka: Pokud používáš Nginx Proxy Manager (NPM) v Dockeru, tento krok
# lze přeskočit nebo upravit, ale pro nativní běh Nginxu je to jistota.
echo "🌐 3/4 Nastavuji základní Nginx routing..."
NGINX_FILE="/etc/nginx/sites-available/default"
sudo bash -c "cat > $NGINX_FILE" <<EOF
server {
    listen 80;
    server_name _;

    # Frontend
    location / {
        root /var/www/portalmcxi/frontend;
        index index.html;
        try_files \$uri \$uri/ /index.html;
    }

    # Backend API Proxy
    location /api/ {
        proxy_pass http://localhost:5000/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }
}
EOF
sudo ln -sf $NGINX_FILE /etc/nginx/sites-enabled/default

# 4. AKTIVACE A RESTART
echo "🔄 4/4 Povoluji služby a provádím daemon-reload..."
sudo systemctl daemon-reload
sudo systemctl enable portalmcxi.service
sudo systemctl enable nginx
sudo systemctl restart nginx

echo "================================================================"
echo "✅ HOTOVO! Prostředí je připraveno."
echo "Nyní můžeš spustit svou Azure Pipeline, která do složek"
echo "nahraje zkompilované soubory a API nastartuje."
echo "================================================================"
