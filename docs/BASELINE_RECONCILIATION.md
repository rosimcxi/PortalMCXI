# PortalMCXI – sloučení základu s MorningInfo

Datum: 2026-10-07. Verze: 1.0.0. Účel: zachovat novější MorningInfo a dokončit opravy importovaného základu.

SOURCE_MAIN=f6f5b4d7de52279c1aff9802ff156b6c3e9f41ac
STATUS=SOURCE_READY_CI_PENDING
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
| .NET build + behaviorální kontroly | PENDING CI; v tomto prostředí chybí SDK |
| RFU kvóta 30 % | PENDING runtime enforcement; žádný Portal RFU job |
| Contabo / production / DB persistence | UNKNOWN; nic nenasazeno |

Staré lokální výsledky v BASELINE_VALIDATION.md nejsou důkazem aktuálního sloučeného backendu. CI výsledek musí být svázán s přesným HEAD pull requestu.

## Co zbývá

RFU projektová kvóta a read-only target audit; ověřená rotace dříve zveřejněných credentials; produkční autentizace a profil; perzistence dat a zpráv; oddělení kolektorů od HTTP čtení; ranní i večerní historie a pylové informace; ověřený deploy/rollback a target test. Současné přímé provider požadavky nejsou dokončením cílové architektury collector → storage → dashboard.
