#!/bin/bash
# ==============================================================================
# Autor:   Ing. Roman Fišer
# Datum:   25.03.2026
# Verze:   2.6.0
# Popis:   Restartovací skript s hloubkovou diagnostikou 404 chyb.
# ==============================================================================

echo "=========================================================="
echo "   RESTART PORTAL MCXI & DIAGNOSTIKA                     "
echo "=========================================================="

echo "1. Restartuji Backend (Systemd)..."
sudo systemctl daemon-reload
sudo systemctl restart portalmcxi.service

echo "2. Restartuji Frontend (PM2)..."
pm2 restart portal-frontend || pm2 start "serve -s /var/www/portalmcxi/frontend -l 5173" --name "portal-frontend"

echo ""
echo "--- KONTROLA VNITŘNÍ ODPOVĚDI (Localhost:5000) ---"
# Test rootu
echo -n "ROOT (/)      : "
curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/
echo ""
# Test system info
echo -n "SYSTEM INFO   : "
curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/api/system/info
echo ""
# Test Scalar
echo -n "SCALAR DOC    : "
curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/scalar/v1
echo ""

echo ""
echo "--- LOGY SLUŽBY (posledních 5 řádků) ---"
sudo journalctl -u portalmcxi.service -n 5 --no-pager

echo ""
echo "--- STAV PORTŮ ---"
sudo ss -tulpn | grep -E '5000|5173'

echo ""
echo "--- VEŘEJNÉ ADRESY ---"
echo "Web:     https://rosimcxi.eu"
echo "API:     https://api.rosimcxi.eu/api/system/info"
echo "Scalar:  https://api.rosimcxi.eu/scalar/v1"
echo "=========================================================="
