# Pravidla vývoje PortalMCXI

## Pracovní tok

1. Načíst skutečný current-main stav, relevantní Issue/Puzzle, soubory, otevřené PR a lokální změny.
2. Zapsat konkrétní cíl, scope, autoritu, závislosti a Definition of Done. Nejprve použít existující komponenty.
3. Izolovat práci od cizích změn. Před každou mutací revalidovat BASE_SHA, EXPECTED_AUTHORITY a vstupní verze.
4. Provést nejmenší opravu a zachovat předchozí funkce. Změna společné komponenty vyžaduje kontrolu dopadu na konzumenty.
5. Spustit relevantní cílený test na exact HEAD; odlišit chybu implementace od chyby testu, prostředí, dostupnosti cíle a runneru. Po opakovaném stejném selhání neprovádět slepé retry.
6. Před commit/push/apply znovu zkontrolovat freshness a konkurenci. Žádný force push, mazání cizích změn či přepis novější práce.
7. Odevzdat celou změnu, test evidence a omezení. Closure až když nezbývá potřebný merge, deploy, test či evidence; ACCEPTED zůstává Romanovi.

## Komponenty a konfigurace

Používejte obecné služby a nad nimi specializované adaptéry. Sdílené logování, storage, identity a AI routing nevytvářejte znovu pro každý modul. Fyzické cesty, hostname, porty, modely a poskytovatele resolveujte přes konfiguraci a kontrakty. RFU fyzické cesty jsou informace o RFU, nejsou výchozími cestami Portalu.

Jedno hlavní .NET API, modulové endpointy/služby a přesné DTO. Kontrakty API, validace a chybové stavy mají být jasné. UI nesmí předstírat dostupná data: zobrazuje čas zdroje, freshness, chybu a případný poslední platný výsledek.

## Sestavení a testy

Existující příkazy projektu:

```bash
dotnet build backend/PortalMCXIBackend/PortalMCXIBackend.csproj -c Release
cd frontend
npm ci
npm run build
npm run lint
```

Tyto příkazy nebyly importem potvrzeny jako úspěšné. RFU běh musí projít projektní kvótou a vzniknout z evidence wrapperu; přímý běh v lokálním vývojovém prostředí není automaticky důkaz z Contabo.

Testovat podle změny: nejprve zdroj/Unit, pak integraci, balík/instalaci a skutečný cíl podle potřeby. TESTED_LOCAL, TESTED_TARGET, provozní stav a ACCEPTED jsou odlišné osy.

## Evidence a dokumentace

Každý běh eviduje RUN_ID, SOURCE_SHA, autoritu/Issue/Puzzle, uzel/executor, začátek/konec, RC, skutečný výsledek, blocker, NEXT_ACTION a odkazy na artefakty. Používat `rfu evidence-run -- <command>` podle ověřeného nainstalovaného kontraktu. Logy redigovat, velké ponechat lokálně; kompaktní výsledek předat přes Shared Result Bus. Cloud pouze stávajícím bounded Storage sync; nebudovat druhého cloud writeru.

Zjišťovací skript vrací jednu sestavu: souhrn nahoře, OK/WARNING/BAD/UNKNOWN, logické sekce a jeden celkový výsledek vhodný pro uložení.

Dokumenty mají vlastníka, stav a odkaz na zdroj. Starší obsah označit HISTORICAL_ONLY/SUPERSEDED; nemažte prokázané znalosti. Aktuální dokument se nesmí tiše zkrátit tak, že ztratí schválené požadavky. Převzaté RFU reference mají připnuté SHA a zůstávají podpůrné.

## Bezpečnost a nasazení

Secrets pouze mimo Git; příklady obsahují prázdné hodnoty/secret refs. Ověřit, že build context ani artefakty nepřenášejí soukromou konfiguraci. Mock login není produkční autentizace.

Před nasazením ověřit target identity, zálohu, rollback, source SHA a aktuální konfiguraci. DB APPLY/DDL/GRANT pouze s explicitním AC. Import a úprava dokumentace neautorizují produkční DB zásah.
