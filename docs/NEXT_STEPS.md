# Doporučené pokračování

## Aktuální pokračování – 7. 10. 2026

MorningInfo a dashboard už jsou na main f6f5b4d. Neimplementovat je znovu. Místní opravy staršího importu jsou reconciliovány se současným modulem: viz [BASELINE_RECONCILIATION.md](BASELINE_RECONCILIATION.md). Nejbližší krok je ověřit CI této změny, potom skutečnou kvótu RFU a cílovou topologii. Persistenci, časované collectory, historii ranních/večerních zpráv a pylové informace teprve dokončit. Aktuální MorningInfo stále sbírá externí data při požadavku; cílové oddělení sběru od čtení není hotové.

## Pořadí

| Pořadí | Malá proveditelná část | Výstup a podmínka dokončení |
|---|---|---|
| 1 – P0 | Ověřit přístupy a rotovat údaje ze ZIPu u poskytovatelů | skutečná rotace bez publikace hodnot; nevydávat očištěný import za rotaci |
| 2 – P0 | Read-only sestava skutečného Contabo přes RFU | hostname/OS/HW, služby, Docker, porty, proxy, source HEAD, CI, DB identity a health; redigovaný souhrn a evidence; jen po splnění quota guardu |
| 3 – P0 | Obnovit sestavitelný backend baseline | opravit rozbité komentáře/deklarace, DTO a mapování; exact-head build PASS a smoke rout |
| 4 – P1 | Ověřit frontend baseline | npm ci/build, sjednotit Tailwind build, odstranit produkční závislost na dev serveru; existující UI zachováno |
| 5 – P1 | V existující RFU autoritě zajistit 30% projektový admission | souběh, vážená kapacita, retry a runtime omezení prokázány; předtím žádné RFU Portal joby |
| 6 – P1 | Jeden read-only RfuAdapter | bounded skutečný Run, source/node provenance a end-to-end výsledek |
| 7 – P1 | Portal perzistence a bezpečný profil uživatele | návrh schématu/migrací, autentizace; DB APPLY teprve po AC |
| 8 – P1 | První Info vertical slice | jeden zdroj počasí + astronomie → uložená data → /api/info/morning → UI s freshness |
| 9 – P2 | Měny a kovy | ověřené zdroje/licence, CZK/jednotky, historie a partial-failure chování |
| 10 – P2 | Osobní části | tatvy podle východu a lokality, validované datum narození, numerologie/kondiciogram označené dle významu |
| 11 – P2 | Sjednocený immutable deploy | potvrzená cílová topologie, záloha/rollback, image SHA, health a skutečný target test |
| 12 – P3 | AI souhrn ranního přehledu | provider-neutral RFU routing, nejlevnější schopný model, 30% účet, citace původních dat; Python/Redis až při doložené potřebě |

Zdrojové opravy lze připravovat izolovaně bez produkční DB změny. RFU práci nelze spustit, dokud není prokázán projektní limit. Nezávislá bezpečná příprava dokumentace pokračuje i při blockeru cíle.

## Zadání nejbližší práce

Dokončit PortalMCXI sestavitelný baseline z aktuálního main. Zachovat původní funkce a dokumentaci, použít docs/ARCHITECTURE_REVIEW.md jako vstupní nálezy, ověřit je na exact HEAD. Neprovádět Contabo deploy ani DB APPLY. Opravit pouze syntaxi, DTO kontrakty a build/frontend pipeline potřebné pro úspěšný lokální build/smoke. Před mutacemi a publikací revalidovat source a konkurenční práci. Výstup: celá změna, source SHA, skutečný test výsledek, známá omezení, další krok. Pokud se používá RFU, nejprve doložit 30% enforcement; jinak RFU dispatch zakázán.

## Otevřené informace

Live Contabo topologie, RFU transport a quota adapter, produkční autentizace, zdroje tržních dat a osobní profil. Křepice/Europe-Prague jsou kontext, ne náhrada ověřených souřadnic; datum narození nelze domýšlet.
