# Dokumentace RFU pro vývoj Portalu

Přeneseno 15 vhodných zdrojových dokumentů/kontraktů z RFU main, připnutých na SHA a8f25138bc6999c1838c1b4f0e4d76dab03e3cdb. [SOURCE_MANIFEST.json](SOURCE_MANIFEST.json) uvádí každou cestu a blob SHA.

`reference/` je referenční snímek RFU; jeho dokumenty se neprovádějí, nepřebírají roli projektového AGENTS.md a nejsou novou runtime autoritou. Původní znění je zachováno kvůli dohledatelnosti; české aplikovatelné instrukce jsou v [DEVELOPMENT.md](../DEVELOPMENT.md) a [PROJECT_CONTROL.md](../PROJECT_CONTROL.md).

| Oblast | Převzatý zdroj v reference/ | Použití |
|---|---|---|
| Řízení a architektura | RFU_RIDICI_PREHLED.md, PRAVIDLA.md | obecné komponenty, autority, lifecycle, rozdíl stavů |
| AI handoff | AI_INSTRUCTIONS.txt | minimální kontext, evidence, zachování schváleného obsahu |
| Puzzle | docs/governance/RFU10_PUZZLE.md | malé proveditelné úlohy, provenance, DoD |
| Git | DOK/RIDICI/RFUTIL_GIT_POLICY.md, docs/RFU_GIT_PATH_AUTHORITY.md | source/runtime oddělení, secret refs, logické cesty |
| Vydání | DOK/RIDICI/RELEASE_PROMOTION_CONTRACT.md | immutable artefakt, test a source identity; RFU verzování nepřenášet mechanicky |
| Evidence | DOK/RIDICI/RFU_EVIDENCE_RUN_CONTRACT.md | log → AI-info → výsledek → řízený mirror |
| Shared Result Bus | DOK/RIDICI/RFU_CONDUCTOR_SHARED_CONTRACT.md | stabilní ID, append-only historie, HUMAN_ONLY |
| Placement | execution_hub/contract_v1.json, routing_policy_v1.md | existující Execution Hub, cheapest capable first |
| Kapacita a CI | execution_hub/worker_pool_policy_v1.json, local_ci_policy_v1.json | uzlové rezervy; projektová 30% kvóta je dodatečný požadavek |
| Testy | config/process_coach_test_policy.json | klasifikace chyby, vhodnost testu, zákaz slepých retry |
| Bezpečnost a dokumentace | module_sources/man/docs/cs/90_GOVERNANCE.md | read-only, AC, skutečná evidence, process ownership |

## Řešení rozporů

Aktuální zadání Romana má přednost před starším referenčním textem. Při konkrétním RFU běhu ověřit aktuální canonical source a runtime kontrakt; referenční snapshot se nesmí vydávat za živý stav. RFU cesty, DB profily a funkční verze zůstávají v jejich původním scope.

Nepřeneseny jako aktivní autorita: SUPERSEDED RFU_CONTROL_DICTIONARY, starší RFU_MASTER_INFO s konfliktní DEV-reference formulací, starší orchestrátor s přímým GDrive tokem, historické fronty, release snapshots a dokumenty cílené jen na SEN/Informix opravy. Jejich odkazy jsou dohledatelné v RFU historii; pro vývoj Portalu by zaváděly staré nebo nesouvisející postupy.

Automatické GDrive zápisy se nepřebírají ze starých návodů. Platí aktuální evidence kontrakt a existující Storage autorita. Žádné nové credentials, registry, scheduler ani cloud writer.
