#!/bin/bash
# ==============================================================================
# Autor:   Ing. Roman Fišer
# Datum:   25.03.2026
# Verze:   2.6.0
# Popis:   Instalátor systémových zkratek pro VPS Contabo (PortalMCXI).
#          Vytváří příkaz 'rp' pro kompletní údržbu a restart.
#          Zajišťuje formátování manuálů pro přehledné zobrazení v terminálu.
# ==============================================================================

echo "?? Čištění systému: Mažu staré verze příkazů a manuálů..."
sudo rm -f /usr/local/bin/rp /usr/local/bin/rfstatus /usr/local/bin/rflog /usr/local/bin/rfman /usr/local/bin/rfportal
sudo rm -f /root/rfman.md /root/rfportal.md

echo "??? Instaluji sadu nástrojů PortalMCXI (verze 2.6.0)..."

# --- 1. MANUÁL (rfman.md) ---
cat > /root/rfman.md << 'EOF'
# ?? RFMAN - Romanův Linuxový Manuál
# Autor: Ing. Roman Fišer | Verze: 2.6.0
---------------------------------------------------------------------------
VLASTNÍ PŘÍKAZY:
- rp        : RESTART VŠEHO (Zastaví/spustí Backend i Frontend + Diagnostika).
- rfstatus  : DASHBOARD (Stav portů, služeb a ID poslední Pipeline).
- rflog     : LIVE LOGY (Sleduje výstup z C# kódu - Ctrl+C pro konec).
- rfportal  : CESTY (Struktura složek a vnitřní síťové propojení).
- rfman     : Tento manuál.

?? MONITOROVACÍ NÁSTROJE (Instalované):
- btop      : Moderní grafický přehled výkonu (CPU, RAM, Síť, Disky).
- htop      : Klasický interaktivní správce procesů.
---------------------------------------------------------------------------
EOF

# --- 2. PŘÍKAZ 'rp' (Restart Portálu) ---
# Tady můžeš kdykoliv změnit cesty nebo logiku restartu
cat > /usr/local/bin/rp << 'EOF'
#!/bin/bash
# Autor: Ing. Roman Fišer | Verze: 2.6.0
echo "=========================================================="
echo "   PORTAL MCXI - FULL RESTART & DIAGNOSTIKA              "
echo "=========================================================="

echo "1. Obnovuji konfiguraci služeb..."
sudo systemctl daemon-reload

echo "2. Restartuji Backend (.NET API)..."
sudo systemctl restart portalmcxi.service

echo "3. Restartuji Frontend (React)..."
pm2 restart portal-frontend || pm2 start "serve -s /var/www/portalmcxi/frontend -l 5173" --name "portal-frontend"

echo ""
echo "--- TEST VNITŘNÍCH ENDPOINTŮ ---"
curl -s -o /dev/null -w "API Root (5000): %{http_code}\n" http://localhost:5000/
curl -s -o /dev/null -w "System Info  : %{http_code}\n" http://localhost:5000/api/system/info

echo ""
echo "--- STAV SLUŽEB ---"
sudo systemctl status portalmcxi.service --no-pager | grep "Active:"
pm2 status portal-frontend

echo ""
echo "=========================================================="
EOF

# --- 3. OSTATNÍ ZKRATKY ---
echo -e "#!/bin/bash\ncat /root/rfman.md" > /usr/local/bin/rfman
echo -e "#!/bin/bash\nsudo journalctl -u portalmcxi.service -f" > /usr/local/bin/rflog

# --- 4. NASTAVENÍ PRÁV ---
chmod +x /usr/local/bin/rp
chmod +x /usr/local/bin/rfman
chmod +x /usr/local/bin/rflog

echo "? Instalace verze 2.6.0 dokončena. Napiš 'rp' pro restart systému."
