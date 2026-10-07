# Oprava sestavitelného základu – 3. 10. 2026

HISTORICAL_TEST_EVIDENCE: Níže uvedené výsledky patří původní místní opravě. Aktuální sloučení s MorningInfo z 7. 10. 2026 a jeho ověření viz [BASELINE_RECONCILIATION.md](BASELINE_RECONCILIATION.md).

Výchozí source: `10ead2e4a22c45aba72759e5fe4fe56d89bb4ef4`.
Změna obnovuje importované moduly a opakovatelný build. Výsledek je lokální vývojový základ; Contabo, DB persistence, produkční autentizace a RFU quota adapter zůstávají neověřené.

## Opraveno

- C# deklarace skryté ve sloučených komentářích, generické kolekce, DTO a JSON source generation.
- Čas na: společný kontrakt taskId/projectId, atomický start/stop a dostupná historie; souběžný start vytvoří jediný aktivní úkol.
- Moduly čas/projekty/úkoly/snippets/monitoring/users jsou propojeny pouze v Development. Production je nezpřístupní ani při `Portal__EnableDemoModules=true`.
- Mock login vrací 501 místo falešného tokenu. Tímto se nezavádí autentizace.
- Endpoint `/api/health`; CORS a důvěryhodné proxy se řídí konfigurací. Nedůvěřovat všem proxy.
- Datum narození přijímá přesné yyyy-MM-dd a odmítá budoucnost. Tatvy mají stále ukázkový východ 06:00; skutečná astronomie je další samostatná práce.
- OpenAPI transitive dependency byla aktualizována na Microsoft.OpenApi 2.7.5 kvůli [GHSA-v5pm-xwqc-g5wc](https://github.com/advisories/GHSA-v5pm-xwqc-g5wc).
- Frontend sestavuje Tailwind lokálně; CDN odstraněno. API se volá relativně nebo přes explicitní VITE_API_BASE_URL. UI označí náhradní odkazy a skutečný výsledek health místo pevného VPS Online.
- Docker frontend používá npm ci, Node 24 a Nginx s `/api/` proxy. Compose zachovává host port 5173 a směřuje jej na kontejnerový port 80. Nasazení této změny nebylo provedeno; externí NPM routing musí být před deployem prověřen.
- Docker build context vynechává lokální konfiguraci, build cache a node_modules.

## Ověření

| Kontrola | Lokální výsledek |
|---|---|
| .NET Release build | OK, 0 chyb, 0 warningů |
| API Development + Production | OK, 41 behaviorálních kontrol |
| Frontend npm ci + build | OK |
| ESLint | OK, 0 chyb/warningů |
| Sestavené CSS a assety | OK, bez Tailwind CDN a development JSX vstupu |
| Nasazení / TESTED_TARGET | neprovedeno |

CI `.github/workflows/baseline.yml` provádí build/test na konkrétním GitHub SHA. Výsledek aktuální revize ověřujte v Actions/checks; tato lokální sestava nenahrazuje CI ani target evidence. GitHub hosted runner je izolovaná projektová CI mimo RFU worker pool. Žádný self-hosted RFU executor, deploy či DB přístup se zde nepoužívá.

Test `tests/smoke_api.py` spouští jen lokální proces API na náhodném loopback portu. Testuje serializaci, routy, validaci data, numerologii/kondiciogram, atomický start/stop, historii a úkolníček; také zákaz demo endpointů v Production. Vytváří jedinou výslednou sestavu RESULT.md a AI_INFO.json, plné logy zůstávají odděleně. Shared Result Bus a řízený cloud mirror mají PENDING, dokud nejsou skutečně napojeny.

## Spuštění

```bash
dotnet build backend/PortalMCXIBackend/PortalMCXIBackend.csproj -c Release
python3 tests/smoke_api.py
cd frontend
npm ci
npm run build
npm run lint
cd ..
python3 tests/check_frontend_build.py
```

Pro běžný lokální vývoj spusťte API v Development na portu 5000 a Vite na 5173. Production ponechává bezpečně demo moduly vypnuté. Backend vyžaduje .NET 10 SDK; frontend Node 24 a přesný package-lock.

## Zachování zdrojů

Kompilovaná autorita je `backend/PortalMCXIBackend/`. Původní `backend/Models`, `backend/Endpoints`, `gen/` a generovací skripty zůstávají historickým podkladem, nejsou součástí tohoto csproj a nesmí přepsat opravené soubory. Refaktor do nové struktury ani odstranění staré dokumentace se neprovádí.

Další krok: ověřit cílovou topologii a 30% RFU quota adapter, následně první Info modul. Produkční osobní/uživatelské funkce vyžadují skutečnou autentizaci a persistence návrh.
