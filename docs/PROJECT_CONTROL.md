# Řídicí dokument PortalMCXI

DOCUMENT_ID=PORTALMCXI-CONTROL
STATUS=CURRENT
DECISION_SOURCE=Roman / zadání 2026-10-03

## Autority a hranice

Roman → ROOT → příslušná domain authority → Issue → Puzzle → Run → Evidence.

GitHub `rosimcxi/PortalMCXI` je zdrojová autorita Portalu. GitHub `rosimcxi/rfu` je zdrojová autorita RFU. Execution Hub rozhoduje o umístění práce; RFU Automation vlastní časování a závislosti, RFU AI poskytovatele/modely, RFU Identity identity a lokální konfiguraci. Portal tyto služby používá, nekopíruje jejich runtime.

RFU completion authority #553 je existující autorita dokončení RFU, není náhradou projektových úloh Portalu. Již CLOSED/MERGED/SUPERSEDED práci znovu neotevírat bez nového doloženého problému.

## Závazný limit

Portal používá maximálně 30 % RFU. Podrobnosti a současná mezera ve vynucení jsou v [RFU_RESOURCE_POLICY.md](RFU_RESOURCE_POLICY.md). Jde o strop, nikoli rezervaci či povinnost spotřebovat 30 %.

## Stav projektu

| Oblast | Stav | Důkaz / omezení |
|---|---|---|
| Zdrojové soubory a dokumentace ZIPu | Import publikován | import 10ead2e; MorningInfo main f6f5b4d; docs/IMPORT.md |
| RFU řídicí dokumentace | Referenční snímek | docs/rfu/SOURCE_MANIFEST.json |
| React/Vite/Tailwind | Ve zdrojích | frontend/package.json |
| .NET 10 | Ve zdrojích | backend/PortalMCXIBackend/PortalMCXIBackend.csproj |
| PostgreSQL 16 | Deklarováno v Compose | skutečný server zatím UNKNOWN |
| Build a základní API | Opravy reconciliovány; CI PENDING | docs/BASELINE_RECONCILIATION.md; target zůstává UNKNOWN |
| Stav Contabo a veřejné služby | UNKNOWN | nutný read-only audit přes RFU |
| Kvóta RFU 30 % | Politika definována; runtime PENDING | bez důkazu vynucení RFU práci nespouštět |
| Nasazení | Neprovedeno tímto importem | DB a produkce se nemění |

## Směr InfoPortalu

Info bude modul téhož Portalu s agregací `/api/info/morning`: počasí, USD/CZK, EUR/CZK, zlato/stříbro v Kč/g, astronomie a osobní informace. Collectory oddělit od čtení dashboardu; historie a freshness se evidují. Odběr dat nesmí vznikat při každém načtení stránky.

Tatvy navázat na skutečný místní východ Slunce. Numerologie, tatvy a kondiciogram jsou osobní/esoterický obsah, nesmějí být prezentovány jako vědecká diagnostika zdraví.

.NET zůstává hlavní business API, PostgreSQL úložiště, React frontend. AI směruje RFU podle schopností, ceny a dostupnosti; Python worker případně interně. Bootstrap ani další backend nepřidávat bez konkrétního důvodu a rozhodnutí.

## Rozcestník

- [Pravidla vývoje](DEVELOPMENT.md)
- [Ověření sestavitelného základu](BASELINE_VALIDATION.md)
- [Limit zdrojů RFU](RFU_RESOURCE_POLICY.md)
- [Převzatá dokumentace RFU](rfu/README.md)
- [Evidence importu](IMPORT.md)
- [Původní dokumentace](../Dokumentace.md)
- [Původní popis nasazení](../PortalNasazeni.md)
- [Projektové instrukce](../AGENTS.md)

Staré návody pro Azure/systemd/PM2/Docker a generovací skripty jsou podklady. Před jejich použitím je nutné ověřit aktuální skutečnost; žádný z nich tímto přenosem nebyl potvrzen jako produkční postup.
