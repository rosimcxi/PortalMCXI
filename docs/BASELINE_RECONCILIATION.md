# PortalMCXI – sloučení základu s MorningInfo

Datum: 2026-10-07. Verze: 1.0.0. Účel: zachovat novější MorningInfo a dokončit opravy importovaného základu.

SOURCE_MAIN=f6f5b4d7de52279c1aff9802ff156b6c3e9f41ac
STATUS=SOURCE_MERGED_TESTED_CI
RFU_DISPATCH=NO
DEPLOY=NO
DATABASE_ACCESS=NO
TESTED_TARGET=NO
ACCEPTED=NO

## Výsledná změna

- Zachovány endpoint `/api/info/morning`, weather/finance/metals služby, osobní výpočty, skutečný místní východ Slunce a celý MorningPanel v dashboardu.
- Obnoveny importované moduly a jejich DTO. Demo moduly pouze v Development; Production je nezpřístupní ani s přepínačem. Login nevydává falešný token.
- Start/stop času je atomický, historie a projectId se vracejí v DTO. Označena ukázková statistika a monitoring.
- Explicitní CORS a trusted proxy konfigurace, health endpoint a přesná validace data včetně odmítnutí budoucnosti.
- Frontend používá lokálně sestavený Tailwind a společný konfigurovatelný API adaptér včetně MorningPanel. Compose frontend je sestavený Nginx; soukromá MorningInfo konfigurace se předává API.
- Jediná baseline CI ověřuje Release build, API Development/Production, syntetický MorningInfo, frontend build/lint a assety. Starý backend-baseline workflow je nahrazen tímto úplným ověřením.
- MorningInfo fixture nevolá externí síť. Ověřuje normalizaci měn, přepočet kovů, počasí/slunce, hranice tatvy, odmítnutí budoucího profilu a částečný výsledek při poškozených provider datech.

## Aktuální důkazy

| Kontrola | Výsledek |
|---|---|
| Frontend build | PASS místně |
| ESLint | PASS místně |
| Sestavené CSS a assety | PASS místně |
| .NET build + behaviorální kontroly | PASS GitHub CI; Development/Production a 9 MorningInfo kontrol |
| RFU kvóta 30 % | PENDING runtime enforcement; žádný Portal RFU job |
| Contabo / production / DB persistence | UNKNOWN; nic nenasazeno |

Staré lokální výsledky v BASELINE_VALIDATION.md nejsou důkazem aktuálního sloučeného backendu. Aktuální test evidence je svázána s přesným HEAD i výsledným main.

## Dokončení source a evidence

PR #4 MERGED: https://github.com/rosimcxi/PortalMCXI/pull/4
SOURCE_HEAD=9a77db607fa2c479dea6d2cd0e84b1d1dee1f576
MERGED_SOURCE_SHA=74b61cd1664314aeb234459f8f69a052c3c62f23
PR CI: https://github.com/rosimcxi/PortalMCXI/actions/runs/37656581244 — SUCCESS.
Main CI: https://github.com/rosimcxi/PortalMCXI/actions/runs/37656849872 — SUCCESS.

Oba běhy ověřily Release build, izolovaný MorningInfo, API Development/Production, npm ci, frontend build, ESLint a výsledné assety. GitHub hosted runner mimo RFU pool. Následná aktualizace těchto dokumentů nemění testované zdrojové soubory.

PR #3 Sbírky zůstává otevřený; vyžaduje reconcile společných souborů, opravu zdrojového literálu `\\n` a produkční autentizaci importu. Jeho schema ani import nebyly spuštěny.

## Hranice autonomního pokračování

V tomto prostředí není RFU runtime ani nastavený SSH profil/klíč pro Contabo. Další target audit potřebuje autorizovaný čtecí kanál nebo předanou redigovanou target sestavu. Nevytvářet novou Queue ani obcházet kvótu přímým executor dispatch.

Čtení RFU current main 7f68641200e590dba52bbf2b203b1a0870ec4619 dne 7. 10. 2026: v execution_hub/capacity.py nebyl nalezen projektový admission limiter pro PortalMCXI. To není úplný runtime audit a nepotvrzuje ani nevylučuje jiné vynucení; stav kvóty zůstává PENDING.

## Co zbývá

RFU projektová kvóta a read-only target audit; ověřená rotace dříve zveřejněných credentials; produkční autentizace a profil; perzistence dat a zpráv; oddělení kolektorů od HTTP čtení; ranní i večerní historie a pylové informace; ověřený deploy/rollback a target test. Současné přímé provider požadavky nejsou dokončením cílové architektury collector → storage → dashboard.
