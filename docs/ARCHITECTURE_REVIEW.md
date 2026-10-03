# Revize návrhu PortalMCXI a InfoPortalu

Podklad: přiložený ZIP a RFU main SHA a8f25138bc6999c1838c1b4f0e4d76dab03e3cdb. Jde o kontrolu zdrojů, nikoli live audit Contabo či úspěšný build.

## Doporučení

Zachovat React/Vite/Tailwind a .NET 10. Začít jedním modulárním backendem a jednou databází Portalu. InfoPortal bude jeho úvodní modul, nikoli nový web. Python/FastAPI, Redis a samostatný AI Gateway proces zatím nevytvářet; nejprve doložit potřebu a existující RFU kontrakty.

.NET služby: MorningInfoService, normalizační collectory, osobní výpočty, perzistence a RfuAdapter. Rozhraní umožní pozdější oddělení workeru bez budování dalšího business backendu.

## Ověřené nálezy

| Priorita | Nález | Důkaz ve zdrojích | Dopad / oprava |
|---|---|---|---|
| P0 | Přístupové údaje ve vstupním ZIPu | PortalMCXI.env, Token*.txt, mackey.txt, Compose, QI.md | import očištěn; rotace u poskytovatelů ještě neprovedena |
| P0 | C# kód sloučen do komentářů | první řádek SystemEndpoints.cs, EsoterikaEndpoints.cs, CasNaEndpoints.cs obsahuje `//` i using/namespace/deklarace | nejprve obnovit správnou strukturu, ověřit kompilací; nelze tvrdit, že endpointy fungují |
| P0 | Nekonzistentní DTO/generika | AppModels.cs má netypované List; CasNa používá CasNaProject/CasNaTask a jiný tvar RunningTask/Request/Response | sjednotit jediný DTO kontrakt a odstranit duplicitní autoritu backend/ vs backend/PortalMCXIBackend/ |
| P1 | PostgreSQL není aplikačně zapojen | csproj neobsahuje EF Core/Npgsql; Program.cs neregistruje DbContext | databáze je deklarovaný kontejner, nikoli ověřená funkce Portalu |
| P1 | Připojeny jen 3 moduly | Program.cs volá System, Esoterika, CasNa | existence dalších souborů neprokazuje dostupné routy |
| P1 | Vývojový frontend v Compose | node:20-alpine, bind mount, npm install + npm run dev | pro produkci immutable build a Nginx; nejprve ověřit target, neprovádět slepý deploy |
| P1 | Dvě cesty Tailwindu | Vite plugin v konfiguraci a CDN skript v index.html | sjednotit CSS build v projektu; nepřidávat Bootstrap |
| P1 | Verzování prostředí je vágní | Node 20 image; lockfile Vite/plugin vyžaduje ^20.19.0 nebo >=22.12.0 | build runtime musí splnit skutečný lockfile; používat npm ci |
| P1 | Údaje UI/API jsou částečně ukázkové | SystemEndpoints má vymyšlené stats; UI fallbacky a FILE_VERSIONS | označit ukázkový stav, ukazovat skutečné freshness/provenance |
| P1 | Tatvy mají fixní východ 06:00 | EsoterikaEndpoints.cs | skutečný východ pro lokalitu a Europe/Prague, včetně přechodů DST |
| P1 | Mock login nenahrazuje autentizaci | UzivateleEndpoints.cs vrací fake token | neveřejné osobní/RFU údaje až po skutečné autentizaci a autorizaci |
| P1 | Generátor může přepsat zdroje | gen/genfile.py zapisuje do relativních backend/frontend cest | historický podklad; před spuštěním reconcile, nikdy slepě nad current source |
| P1 | Staré CI obsahuje mutační deploy | azure-pipelines.yml kopíruje přes SSH, rm -rf a restartuje Compose | v GitHubu nevytváříme aktivní deploy workflow; případné externí Azure napojení nutno zjistit |
| P1 | 30% projektová kvóta není prokázána | prověřená RFU capacity.py řeší uzly, ne project_id | před RFU dispatch nutná evidence vynucení |

## Lepší hranice služeb

| Vrstva | Vlastní | Nepřebírá |
|---|---|---|
| React | zobrazení, uživatelské preference, dashboard | provider klíče, collectory a plánování |
| .NET Portal | business API, oprávnění, normalizace, čtení dat | RFU placement a další registry |
| PostgreSQL Portalu | normalizované pozorování, preference, historie | RFU operational registry jako druhý zdroj pravdy |
| RfuAdapter | verzovaný veřejný kontrakt request/result, mapování projektových ID | přímé čtení interní SQLite či libovolný remote shell |
| RFU | plánování, placement, HW/AI, run/evidence, storage | business význam cen či osobních výpočtů Portalu |

## Data místo dvaceti synchronních externích volání

GET `/api/info/morning` vrací poslední platná uložená data. Každá sekce má source, observedAt, fetchedAt, validUntil, status a případnou chybu; částečný výpadek nedělá z celého dashboardu falešné BAD/OK. UTC pro uložené časy, explicitní Europe/Prague pro den a osobní výpočty.

Kurzy vždy explicitně CZK za 1 USD/EUR a datum kurzu. Kov uvádí spot/reference hodnotu, měnu a Kč/g; výkupní cenu a marži neodvozovat jako totéž. Uchovat původní jednotku, převod i source. Vývoj proti včerejšku či 7/30denní trend zobrazit až po získání srovnatelné historie.

Externí data sbírat řízeně přes existující RFU automatizaci, s cache v PostgreSQL a rate-limit/backoff. Intervaly musí odpovídat aktualizaci konkrétního zdroje a jeho licenci. Zdroj počasí/kovů/měn se vybere a ověří v samostatném kroku; tímto návrhem se neslibuje cena ani dostupnost provideru.

## Co je návrh a co implementace

MorningInfoService, collectory, RfuAdapter a endpoint `/api/info/morning` jsou cílový návrh. Import je nezavedl. EF Core, Python AI worker a Redis nejsou prokázané současné součásti. Živý stav Contabo zůstává UNKNOWN.
