# Přenos PortalMCXI do GitHubu

- Cíl: https://github.com/rosimcxi/PortalMCXI
- Zdroj: PortalMCXI (1).zip
- Datum: 3. 10. 2026
- Zachována původní struktura backend, frontend, gen, scripts a dokumentace.
- Neprovádí se nasazení, DB změna ani připojení na Contabo.
- Zdrojový stav může obsahovat neúplné či nefunkční části; sestavení nebylo potvrzeno.
- Původní Azure pipelines byly přeneseny jako zdrojové soubory; v GitHubu se automaticky nespouštějí. Existující externí Azure integrace nebyla ověřena.
- Nastavení připojení repozitáře v rozhraní ChatGPT projektu tímto importem není změněno.

## Bezpečnostní úpravy

Vynechány tokeny, klíčový soubor, soukromá konfigurace a IDE cache:

- `PortalMCXI.env`
- `Token.txt`
- `Token2.txt`
- `backend/PortalMCXIBackend/PortalMCXIBackend.csproj.lscache`
- `mackey.txt`

Skutečná hesla odstraněna i z případných dalších výskytů. Docker Compose načítá DB_PASSWORD z prostředí a vyžaduje neprázdnou hodnotu. Přidány .env.example, PortalMCXI.env.example a pravidla .gitignore. Žádný původní token, klíč ani heslo není součástí importu. Pokud byly skutečně používány, je potřeba je rotovat u příslušného poskytovatele; rotace nebyla tímto přenosem provedena.

Upravené původní soubory:

- `.gitignore`
- `QI.md`
- `README.md`
- `docker-compose.yml`

## Kódování

Následující soubory byly převedeny z Windows-1250 do UTF-8 pro zachování českého textu:

- `Postgres_install.sh`
- `Projekt_vytvor_adresare.sh`
- `README.md`
- `StartPortalMCXI.sh`
- `dir.txt`
- `frontend/src/Log.jsx`
- `gen/project_data.json`
- `scripts/portal_info.sh`
- `scripts/rf_install.sh`
- `scripts/rp.sh`
- `scripts/symlinky.md`

## Další ověření

Read-only audit skutečného Contabo přes RFU; kontrola zdrojového SHA a konfigurace; sestavení frontendu a backendu; teprve potom návrh sjednoceného nasazení a rozvoj InfoPortalu.
