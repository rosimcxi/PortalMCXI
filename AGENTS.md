# Instrukce pro vývoj PortalMCXI

Platí pro celý repozitář. Před změnou přečtěte `docs/PROJECT_CONTROL.md`, `docs/DEVELOPMENT.md`, `docs/RFU_RESOURCE_POLICY.md` a `docs/rfu/README.md`.

- Zadání a rozhodnutí Romana mají přednost. Zachovávejte již schválené funkce i dokumentaci.
- Canonical source Portalu je `rosimcxi/PortalMCXI`; RFU zůstává v `rosimcxi/rfu`.
- React/Vite/Tailwind a .NET 10 jsou existující základ. Python přidávejte jako specializovaný AI worker až při doložené potřebě.
- Nevytvářejte druhý scheduler, Queue, Node/AI Registry, Storage Router ani placement authority. Používejte existující RFU kontrakty.
- Portal smí spotřebovat nejvýše 30 % zdrojů RFU. Bez prokázaného vynucení kvóty se RFU práce Portalu nespouští; neobcházejte limit přímým runnerem či AI voláním.
- Před claim, zápisem a commit/push/apply znovu ověřte aktuální HEAD, autoritu, stav úlohy a konkurenční změny. Stale práci reconcile/rebase nebo označte SUPERSEDED.
- Postup: evidence → příčina → nejmenší oprava → cílený test → evidence.
- Žádné skutečné tajné údaje v Gitu, logu, AI vstupu ani dokumentaci; používejte secret refs a soukromou konfiguraci.
- DB APPLY/DDL/GRANT a provozní změny přes RFU vyžadují explicitní platné AC; příprava zdrojů a read-only kontrola je oddělená.
- ACCEPTED je pouze Roman; TESTED_TARGET vyžaduje skutečný důkaz z cíle. Commit, build a návrh nejsou nasazení.
- Diagnostika tvoří jednu sestavu s úvodním souhrnem, stavy OK/WARNING/BAD/UNKNOWN, logickými sekcemi a závěrečným výsledkem.
- Log → redigované AI-info → Shared Result Bus → existující řízený cloud mirror. Velké logy zůstávají lokálně.
- Komunikujte česky, stručně a pravdivě. Při předání změn poskytujte celé změněné soubory či celý výsledný projekt, ne fragmenty.

Převzaté RFU dokumenty v `docs/rfu/reference/` jsou dohledatelné referenční snímky. Jejich RFU cesty, verze, konkrétní DB profily a historická rozhodnutí automaticky nepřenášejte do Portalu.
