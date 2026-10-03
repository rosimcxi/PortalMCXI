#!/bin/bash
# Soubor: /root/info_portal.sh
# Autor: Ing. Roman Fišer
# Účel: Zobrazení dokumentace, nápovědy a export stavu systému.

LOG_FILE="/root/portal_info.log"
NOW=$(date +"%Y-%m-%d %H:%M:%S")

# Vše v tomto bloku se vypíše na obrazovku a zároveň uloží do LOG_FILE
{
echo "================================================================"
echo " ?? PORTALMCXI - INFORMAČNÍ A DIAGNOSTICKÝ PANEL"
echo " ?? Čas: $NOW"
echo "================================================================"
echo ""

# --- 1. POPIS ARCHITEKTURY ---
echo "--- ??? JAK TO MÁ BÝT (ARCHITEKTURA) ---"
echo "1. Frontend (React):   /var/www/portalmcxi/frontend"
echo "2. Backend (.NET 10):  /var/www/portalmcxi/api (Soubor: PortalMCXIBackend.dll)"
echo "3. Služba API:         portalmcxi.service (Běží na portu 5000)"
echo "4. Web Server:         Nginx (Naslouchá na portu 80, směruje na / a /api/)"
echo "5. Nasazování:         Azure DevOps Pipeline automaticky kopíruje soubory."
echo ""

# --- 2. NÁPOVĚDA A PŘÍKAZY ---
echo "--- ?? HELP / UŽITEČNÉ PŘÍKAZY ---"
echo "- Stav API:            systemctl status portalmcxi.service"
echo "- Restart API:         systemctl restart portalmcxi.service"
echo "- Živé logy API:       journalctl -u portalmcxi.service -f"
echo "- Editace API služby:  nano /etc/systemd/system/portalmcxi.service"
echo "- Restart Webserveru:  systemctl restart nginx"
echo "- Kontrola portů:      netstat -tulpn | grep -E '5000|80'"
echo ""

# --- 3. AKTUÁLNÍ STAV SYSTÉMU ---
echo "--- ?? NAČÍTÁM AKTUÁLNÍ INFORMACE O SYSTÉMU ---"

# Stav služeb
API_STATUS=$(systemctl is-active portalmcxi.service)
NGINX_STATUS=$(systemctl is-active nginx)

if [ "$API_STATUS" = "active" ]; then
    echo "? Backend API (portalmcxi.service): BĚŽÍ"
else
    echo "? Backend API (portalmcxi.service): NEBĚŽÍ ($API_STATUS)"
fi

if [ "$NGINX_STATUS" = "active" ]; then
    echo "? Web Server (Nginx):               BĚŽÍ"
else
    echo "? Web Server (Nginx):               NEBĚŽÍ ($NGINX_STATUS)"
fi

# Kontrola portu 5000
PORT_5000=$(netstat -tulpn 2>/dev/null | grep :5000)
if [ -n "$PORT_5000" ]; then
    echo "? Port 5000 (API):                  NASLOUCHÁ"
else
    echo "? Port 5000 (API):                  NENASLOUCHÁ (API pravděpodobně spadlo)"
fi

# Zjištění metadat z Azure (pokud existují)
if [ -f "/var/www/portalmcxi/api/deploy-info.json" ]; then
    echo "?? Poslední nasazení (Azure):        $(cat /var/www/portalmcxi/api/deploy-info.json)"
fi
echo ""

# --- 4. VÝPIS LOGŮ ---
echo "--- ?? POSLEDNÍCH 15 ŘÁDKŮ Z LOGU API (JOURNALCTL) ---"
journalctl -u portalmcxi.service -n 15 --no-pager
echo ""
echo "================================================================"
echo "?? Kompletní výpis tohoto panelu byl uložen do: $LOG_FILE"
echo "================================================================"

} | tee $LOG_FILE
