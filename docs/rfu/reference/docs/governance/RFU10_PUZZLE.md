Ahoj Romane,

připravil jsem z toho **nejvyšší řídicí dokument RFUTIL 10 / Puzzle**. Protože přesun do Work režimu nebyl přijat, nemohu v tomto tahu fyzicky zapsat soubor do pracovního stromu RFU. Níže ale předávám **celý kanonický dokument**, připravený k uložení například jako:

`docs/governance/RFU10_PUZZLE.md`

# RFUTIL 10 – Puzzle Architecture & Governance Standard

## Metadata dokumentu

| Položka | Hodnota |
|---|---|
| Document-ID | RFU-GOV-PUZZLE-001 |
| Název | RFUTIL 10 – Puzzle Architecture & Governance Standard |
| Verze dokumentu | 1.0.0 |
| Stav | DRAFT |
| Úroveň | HIGHEST GOVERNING DOCUMENT |
| Produkt | RFUTIL |
| Hlavní verze | 10 |
| Owner | RFUTIL Project |
| Technical Owner | Roman Fišer |
| Approved-By | PENDING |
| Effective-From | po schválení |
| Last-Review | 2026-08-12 |
| Next-Review | před prvním RFUTIL 10 release candidate |
| Supersedes | dílčí návrhy RFUTIL 9 / Puzzle |
| Compatibility | RFUTIL 10+ |
| SHA-256 | generuje release/get mechanismus |

---

# 1. Účel dokumentu

Tento dokument je **nejvyšším řídicím dokumentem architektury Puzzle systému RFUTIL 10**.

Definuje závazná pravidla pro:

- Puzzle,
- jejich zadání a řešení,
- lifecycle,
- stavy,
- SQLite evidenci,
- funkce vznikající z Puzzle,
- verze funkcí,
- buildy a manifesty,
- testování,
- schvalování,
- Feedback/FB,
- dokumentaci,
- Git,
- export a import,
- recovery,
- AI handoff,
- práci různých AI systémů,
- reprodukovatelnost,
- auditovatelnost.

Nižší dokumentace, implementace ani modulové standardy nesmí být s tímto dokumentem v rozporu.

V případě konfliktu má tento dokument přednost.

---

# 2. Základní princip RFUTIL 10

RFUTIL 10 používá Puzzle jako jednotnou evidenci:

- požadavků,
- problémů,
- návrhů,
- rozhodnutí,
- implementace,
- testů,
- dokumentace,
- výsledků,
- historie,
- AI kontextu.

Puzzle není pouze ToDo.

Puzzle představuje **auditovatelnou jednotku změny a znalosti projektu**.

Základní tok:

```text
Požadavek
   |
   v
Puzzle
   |
   v
Analýza
   |
   v
Návrh řešení
   |
   v
Implementace
   |
   v
Funkce / změna
   |
   v
Test
   |
   v
Feedback
   |
   v
Schválení
   |
   v
Realized
   |
   v
Manifest / Build / Release
```

---

# 3. Kanonický zdroj pravdy

## RFU-PUZ-001 — SQLite je kanonická evidence

**MUST**

Kanonickým strukturovaným zdrojem Puzzle evidence je modulová nebo centrální SQLite databáze RFUTIL podle výsledného technického návrhu.

SQLite musí uchovávat minimálně:

- Puzzle ID,
- typ,
- název,
- modul,
- zadání,
- verzi zadání,
- řešení,
- verzi řešení,
- atributy,
- stav,
- active,
- realized,
- priority,
- dependency,
- historii,
- vazby na funkce,
- vazby na verze,
- vazby na build,
- testy,
- schválení,
- Feedback summary,
- Git metadata,
- AI metadata.

---

## RFU-PUZ-002 — SQLite nesmí být jedinou obnovitelnou reprezentací

**MUST**

Puzzle musí mít exportovatelnou adresářovou reprezentaci vhodnou pro:

- Git,
- člověka,
- AI,
- recovery,
- migraci.

Poškození nebo ztráta SQLite nesmí znamenat nenávratnou ztrátu Puzzle evidence.

---

# 4. Identita Puzzle

Každé Puzzle musí mít stabilní ID.

Například:

```text
PUZZLE-000001
PUZZLE-000002
PUZZLE-000003
```

## RFU-PUZ-003 — ID je neměnné

**MUST**

Jednou přidělené Puzzle ID:

- nesmí změnit význam,
- nesmí být znovu použito,
- nesmí být přečíslováno.

---

# 5. Základní stavové atributy

Puzzle musí mít minimálně:

```text
active
realized
status
```

---

## 5.1 active

```text
active = A | N
```

Význam:

### A

Puzzle je aktuálně platné.

### N

Puzzle není aktuálně aktivní.

Například:

- zrušené,
- nahrazené,
- archivované,
- historické.

---

## 5.2 realized

```text
realized = A | N
```

### realized=N

Puzzle ještě nesplnilo všechny požadavky potřebné k prohlášení za realizované.

### realized=A

Puzzle:

1. bylo implementováno,
2. bylo zahrnuto do příslušné verze/buildu,
3. prošlo požadovanými testy,
4. bylo ověřeno,
5. bylo schváleno,
6. má potřebnou dokumentaci podle pravidel dané změny.

---

## RFU-PUZ-004 — realized=A nesmí být deklarativní příznak

**MUST**

`realized=A` musí být odvoditelné z auditovatelných důkazů.

Minimálně:

```text
realized_version
realized_build
realized_at
realized_by
test_status
approval_status
```

Podle typu Puzzle mohou být vyžadovány další důkazy.

---

# 6. Workflow status

`active` a `realized` nenahrazují detailní lifecycle.

Puzzle proto musí obsahovat také workflow `status`.

Doporučené hodnoty:

```text
NEW
ANALYSIS
READY
IMPLEMENTING
TESTING
WAITING_APPROVAL
BLOCKED
REALIZED
SUPERSEDED
CANCELLED
ARCHIVED
```

Jednotné provozní stavové hodnoty výsledků zůstávají:

```text
OK
WARNING
BAD
SKIPPED
ERROR
```

---

# 7. Puzzle lifecycle

Typický lifecycle:

```text
NEW
 |
 v
ANALYSIS
 |
 v
READY
 |
 v
IMPLEMENTING
 |
 v
TESTING
 |
 v
WAITING_APPROVAL
 |
 v
REALIZED
```

Alternativní větve:

```text
BLOCKED
CANCELLED
SUPERSEDED
ARCHIVED
```

Každý přechod musí být auditovatelný.

---

# 8. Verzované zadání

## RFU-PUZ-005 — Zadání je verzované

**MUST**

Změna zadání nesmí přepsat historické zadání.

Puzzle musí umožnit:

```text
Requirement v1
Requirement v2
Requirement v3
...
```

Každá verze musí mít minimálně:

```text
version
created_at
created_by
change_reason
content
```

---

# 9. Verzované řešení

Stejný princip platí pro návrh řešení.

Puzzle musí být schopno uchovat:

```text
Solution v1
Solution v2
Solution v3
```

Tím lze zpětně určit:

- co bylo původně navrženo,
- co se změnilo,
- proč se to změnilo,
- která varianta byla implementována.

---

# 10. Puzzle → Function

Puzzle může vytvářet:

- novou funkci,
- novou verzi funkce,
- změnu existující funkce,
- konfiguraci,
- dokumentaci,
- test,
- databázovou změnu,
- infrastrukturu,
- jiný projektový artefakt.

Vazba musí být explicitní.

```text
Puzzle
   |
   +--> Function
           |
           +--> Function Version
```

---

# 11. Stav Puzzle a stav funkce

## RFU-PUZ-006 — Lifecycle Puzzle a Function musí být provázán

**MUST**

Stav Puzzle a výsledné funkce se nesmí bez vysvětlení rozcházet.

Například:

```text
Puzzle:
status=TESTING
realized=N

Function:
status=TESTING
approved=N
```

Po dokončení:

```text
Puzzle:
status=REALIZED
realized=A

Function:
status=SUPPORTED
approved=A
```

Pokud se stavy odlišují, musí být evidován explicitní důvod.

---

# 12. Puzzle → Function → Manifest

Manifest nesmí být ručně vytvořenou optimistickou reprezentací.

Musí vycházet ze skutečného stavu:

```text
Puzzle
   |
Function
   |
Function Version
   |
Tests
   |
Approval
   |
Manifest
```

## RFU-PUZ-007 — Manifest odráží skutečnost

**MUST**

Manifest musí být generován nebo validován proti:

- Puzzle,
- Function registry,
- Function version,
- testům,
- build evidence,
- approval evidence.

---

# 13. Dokumentace jako Puzzle

Dokumentace může být:

1. součást hlavního Puzzle,
2. samostatné Puzzle.

Příklad:

```text
PUZZLE-0103  Implementace get AI
PUZZLE-0104  Test get AI
PUZZLE-0105  Dokumentace get AI
```

Dependency:

```text
0104 depends_on 0103
0105 depends_on 0103
```

---

## RFU-PUZ-008 — Implementace automaticky neznamená hotovou dokumentaci

**MUST**

Pokud typ změny vyžaduje dokumentaci, nesmí být absence dokumentace skryta příznakem hotové implementace.

Dokumentační Puzzle může být dependency před:

```text
realized=A
```

nebo před povýšením release role.

---

# 14. Feedback / FB

Puzzle musí mít dostupné shrnutí výsledků Feedback systému.

Detailní Feedback zůstává v samostatné historii.

Puzzle obsahuje agregované údaje například:

```text
fb_status
fb_last_run
fb_errors
fb_warnings
fb_summary
fb_recommendation
fb_open_items
```

---

## RFU-PUZ-009 — Feedback summary

**MUST**

Z Puzzle musí být bez načítání celé FB historie zjistitelné:

- poslední relevantní výsledek,
- hlavní chyby,
- hlavní warningy,
- doporučení,
- otevřené body,
- důvod, proč Puzzle případně není realized.

---

# 15. Git reprezentace Puzzle

Puzzle musí být exportovatelné do člověkem i strojem čitelného Git stromu.

Příklad:

```text
puzzle/
└── PUZZLE-000123/
    ├── puzzle.json
    ├── requirement.md
    ├── solution.md
    ├── attributes.json
    ├── dependencies.json
    ├── history.json
    ├── tests.json
    ├── feedback.json
    ├── implementation.md
    └── manifest.json
```

---

# 16. Git není kanonická runtime databáze

Git je:

- auditní vrstva,
- versioning,
- recovery zdroj,
- distribuční reprezentace,
- podklad pro člověka,
- podklad pro AI.

SQLite zůstává kanonickým strukturovaným runtime registrem Puzzle.

---

# 17. Import Puzzle

RFUTIL musí podporovat opačný směr:

```text
adresář/Git -> SQLite
```

Typické použití:

### Recovery

Obnova SQLite z Git reprezentace.

### Nové Puzzle

AI nebo člověk připraví validní adresář nového Puzzle.

RFUTIL jej ověří a naimportuje.

---

## RFU-PUZ-010 — Import nesmí destruktivně přepisovat data

**MUST**

Import musí ověřit:

- schema,
- Puzzle ID,
- verzi,
- atributy,
- dependencies,
- historii,
- existující SQLite záznam,
- novější verzi,
- konflikt.

Novější kanonická data nesmí být tiše přepsána starším importem.

---

# 18. Recovery

Musí být možné rekonstruovat Puzzle evidenci z adresářové/Git reprezentace.

Recovery proces musí být:

- deterministický,
- auditovatelný,
- validovaný,
- bezpečný proti ztrátě novějších dat.

---

# 19. Get engine

RFUTIL 10 zavede společný mechanismus pro získávání reprezentací projektu.

Hlavní funkce:

```text
get src
get puzzle
get AI
```

Tyto funkce mají používat společné:

- scope resolvery,
- registry,
- SQLite data,
- Git informace,
- manifesty,
- export mechanismy.

Nesmějí vznikat paralelní proprietární implementace pro jednotlivé AI nebo moduly.

---

# 20. `get src`

Účel:

Připravit skutečné implementační zdroje RFUTIL určené pro Git.

Příklad:

```bash
rfu get src
```

Scope může být rozšířen podle implementace.

---

# 21. `get puzzle`

Účel:

Exportovat Puzzle evidenci do standardní adresářové reprezentace.

```bash
rfu get puzzle
```

Výstup musí obsahovat:

- aktuální data,
- atributy,
- verzované zadání,
- verzované řešení,
- dependencies,
- historii,
- testy,
- Feedback summary,
- vazby na funkce,
- manifestová metadata.

---

# 22. `get AI`

Účel:

Vytvořit kompletní AI handoff potřebný k pokračování práce jiným AI systémem.

Podporované scope minimálně:

```text
celé RFU
modul
next Puzzle
Puzzle ID
```

Například:

```bash
rfu get AI --scope rfu
rfu get AI --scope module:backup
rfu get AI --scope next
rfu get AI --scope puzzle:123
```

Konkrétní finální CLI syntaxe může být upravena v implementačním Puzzle, ale schopnosti jsou závazné.

---

# 23. AI vendor neutrality

## RFU-AI-001 — AI handoff nesmí být závislý na jednom poskytovateli

**MUST**

AI handoff musí být použitelný minimálně pro:

- ChatGPT,
- Codex,
- Gemini,
- jiné cloudové AI,
- lokální code modely,
- budoucí kompatibilní modely.

Preferované základní formáty:

```text
Markdown
JSON
zdrojové soubory
```

---

# 24. Obsah AI handoff

AI balíček musí podle scope obsahovat relevantní část z:

- identifikace RFUTIL,
- verze,
- buildu,
- scope,
- aktuálního Puzzle,
- zadání,
- historie zadání,
- návrhu řešení,
- historie řešení,
- dependencies,
- blocking items,
- relevantních funkcí,
- relevantního source code,
- architektonických pravidel,
- projektových rozhodnutí,
- konfigurace bez secretů,
- testů,
- výsledků testů,
- Feedback,
- dokumentace,
- známých chyb,
- otevřených bodů,
- acceptance criteria,
- Definition of Done,
- omezení změn.

---

# 25. AI_HANDOFF

Preferovaný balíček obsahuje hlavní vstup:

```text
AI_HANDOFF.md
```

a podle potřeby další strom:

```text
context/
sources/
tests/
history/
metadata/
```

Jednodušší AI může použít pouze `AI_HANDOFF.md`.

Pokročilejší AI může použít celý strom.

---

# 26. AI auto

`get AI` musí být navrženo s podporou režimu:

```text
auto
```

Auto režim automaticky určí relevantní kontext na základě:

- scope,
- Puzzle,
- dependencies,
- funkcí,
- historie,
- změněných zdrojů,
- testů,
- rozhodnutí.

---

# 27. AI compress

`get AI` musí být navrženo s podporou:

```text
compress
```

Účelem je vytvořit zhuštěnou reprezentaci pro modely s omezeným context window.

Kompresní mechanismus nesmí zničit dohledatelnost původních zdrojů.

Preferovaný princip:

```text
compact summary
      |
      +--> reference
              |
              +--> full source
```

---

# 28. AI nástroje musí používat společné RFUTIL služby

## RFU-AI-002

**MUST**

AI vrstva nesmí vytvářet vlastní:

- Git resolver,
- SQLite resolver,
- Puzzle loader,
- configuration loader,
- comparison engine,
- export engine.

Musí používat stejné kanonické služby jako RFUTIL.

---

# 29. Git repository

RFUTIL musí znát kanonickou cestu k RFU Git pracovnímu stromu.

Cesta nesmí být libovolně duplikována v každém modulu.

Musí být řízena centrální konfigurací/SQLite registry.

---

# 30. `--commit`

Funkce:

```text
get src
get puzzle
get AI
```

musí podporovat řízený režim:

```text
--commit
```

Například:

```bash
rfu get src --commit
rfu get puzzle --commit
rfu get AI --commit
```

---

## 30.1 Bez `--commit`

Operace pouze:

- připraví export,
- validuje data,
- vytvoří artefakty.

Git commit nevytváří.

---

## 30.2 S `--commit`

Operace:

1. zjistí kanonický Git repository,
2. ověří repository,
3. zjistí branch/revision,
4. zkontroluje pracovní strom,
5. vytvoří/generuje výstup,
6. uloží jej do správného adresáře,
7. provede validaci,
8. ukáže změny,
9. vytvoří řízený commit.

Push je samostatná politika a nesmí být implicitně zaměňován s commit.

---

# 31. Git funkce RFUTIL

RFUTIL musí mít společné Git služby minimálně pro:

### Stav

```text
git status
```

### Verze/revision

```text
git version
```

Musí být možné zjistit minimálně:

- repository,
- branch,
- commit,
- dirty/clean,
- případně tag/release vazbu.

### Compare source

Porovnání:

```text
aktuální source tree
vs
Git
```

### Compare Puzzle

Porovnání:

```text
SQLite Puzzle
vs
Git Puzzle export
```

---

# 32. Výsledky Git/Puzzle compare

Compare musí být schopno rozlišit minimálně:

```text
SAME
NEW
MISSING
CHANGED
OUTDATED
CONFLICT
```

Musí umět zjistit:

- chybějící artefakty,
- přebývající artefakty,
- změněné artefakty,
- starší export,
- konflikt verzí.

---

# 33. Source export

Kompletní předávací Git strom může obsahovat například:

```text
RFU_SOURCE/
├── src/
├── puzzle/
├── ai/
├── docs/
└── metadata/
```

---

# 34. Tři hlavní informační stromy

RFUTIL 10 rozlišuje:

## `src/`

Implementace.

## `puzzle/`

Kanonická exportní reprezentace požadavků, řešení, historie a stavů.

## `ai/`

AI-optimalizovaný informační pohled.

Tyto stromy nesmějí být zaměňovány, ale mohou se vzájemně odkazovat.

---

# 35. Reprodukovatelnost

## RFU-GOV-001

**MUST**

`get src`, `get puzzle` a `get AI` musí být reprodukovatelné.

Stejný kanonický stav má při stejných podmínkách vytvořit ekvivalentní obsah.

Nestabilní runtime údaje musí být odděleny od obsahově deterministických dat.

---

# 36. Auditovatelnost

Každý významný export musí obsahovat metadata například:

```text
generated_at
RFUTIL_version
source_revision
SQLite_schema_version
scope
Puzzle IDs
function versions
generator_version
```

A tam, kde je vhodné:

```text
SHA-256
```

---

# 37. Secrety

## RFU-SEC-001

**MUST**

Do:

- Puzzle exportu,
- Git exportu,
- AI handoff,
- dokumentace,
- logů,

nesmějí být exportována čitelná hesla, tokeny ani jiné secrets.

Povolené jsou bezpečné reference například:

```text
secret_ref
credential_id
vault_reference
```

---

# 38. Next Puzzle

RFUTIL musí být schopný určit další vhodné Puzzle.

Typický filtr:

```text
active=A
realized=N
not blocked
```

Výběr může dále zohlednit:

1. priority,
2. dependencies,
3. explicitní pořadí,
4. připravenost,
5. předchozí Puzzle.

---

## RFU-PUZ-011 — Next musí být vysvětlitelné

**MUST**

Automaticky vybrané následující Puzzle musí obsahovat důvod výběru.

Například:

```text
NEXT_PUZZLE=PUZZLE-000157

REASON:
ACTIVE=A
REALIZED=N
PRIORITY=P0
BLOCKED=N
ALL_REQUIRED_DEPENDENCIES_REALIZED=A
```

---

# 39. Dependency

Puzzle mohou být vzájemně závislé.

Vazba musí být explicitní.

Například:

```text
depends_on
blocks
related_to
supersedes
documentation_for
test_for
```

Přesný registry enum bude definován v příslušném registru RFUTIL.

---

# 40. Definition of Done Puzzle

Puzzle lze označit `realized=A` pouze tehdy, když splňuje svůj Definition of Done.

Obecný DoD:

- [ ] Zadání je aktuální a verzované.
- [ ] Řešení je evidované.
- [ ] Dependencies jsou vyřešené nebo explicitně akceptované.
- [ ] Implementace odpovídá schválenému zadání.
- [ ] Relevantní testy byly provedeny.
- [ ] Test evidence existuje.
- [ ] Feedback neobsahuje blokující chybu.
- [ ] Dokumentace je dokončena, pokud je vyžadována.
- [ ] Manifest odpovídá skutečnému stavu.
- [ ] Build/version vazba existuje.
- [ ] Schválení existuje.
- [ ] `realized=A` je auditovatelně doložitelné.

---

# 41. Úrovně ověření

RFUTIL používá standardní úrovně:

```text
CREATED
STATIC_OK
TESTED_LOCAL
TESTED_TARGET
ACCEPTED
```

Puzzle musí podle typu změny definovat požadovanou cílovou úroveň.

`realized=A` nesmí být nastaveno před dosažením požadované úrovně.

---

# 42. Typy Puzzle

Puzzle systém musí umožnit minimálně kategorie jako:

```text
FEATURE
BUG
ARCHITECTURE
DATABASE
CONFIGURATION
TEST
DOCUMENTATION
SECURITY
REFACTOR
MIGRATION
RELEASE
RESEARCH
AI_ANALYSIS
RECOVERY
```

Seznam je rozšiřitelný přes registry.

---

# 43. Dokumentační governance

Tento dokument je nadřazen:

- modulovým Puzzle dokumentům,
- AI promptům,
- implementační dokumentaci,
- Git export dokumentaci,
- jednotlivým ToDo.

Změna zásad v tomto dokumentu musí být sama řízena verzovanou změnou.

---

# 44. Stabilní Rule-ID

Pravidla uvedená tímto dokumentem mají stabilní Rule-ID.

Například:

```text
RFU-PUZ-001
RFU-PUZ-002
RFU-AI-001
RFU-GOV-001
RFU-SEC-001
```

Význam existujícího Rule-ID se nesmí změnit.

Pokud je potřeba jiné pravidlo, vytvoří se nové ID.

---

# 45. Výjimky

Výjimka z MUST pravidla musí obsahovat minimálně:

```text
Rule-ID
Reason
Owner
Approved-By
Valid-From
Valid-To
Risk
Mitigation
```

Výjimka nesmí být evidována pouze odkazem na číslo kapitoly.

---

# 46. Compatibility

Každé realizované Puzzle ovlivňující runtime musí podle relevance uvádět:

- backward compatibility,
- config migration,
- database migration,
- restart,
- downtime,
- rollback,
- minimum version,
- dependency change.

---

# 47. Build evidence

U realizovaného Puzzle musí být možné dohledat odpovídající build evidence.

Například:

```text
Puzzle ID
Function ID
Function Version
RFUTIL Version
Build ID
Commit
Test Report
Approval
SHA-256
```

---

# 48. Recovery evidence

Import/recovery musí vytvořit report obsahující minimálně:

```text
source
target
Puzzle count
new
updated
same
conflict
rejected
errors
SHA-256 / source revision
```

---

# 49. Import nového Puzzle vytvořeného AI

AI může připravit nové Puzzle v předepsané adresářové podobě.

Takové Puzzle je pouze návrh.

Při importu musí RFUTIL:

1. validovat formát,
2. přidělit nebo validovat ID,
3. ověřit attributes,
4. ověřit dependencies,
5. zařadit jej do SQLite,
6. zaznamenat původ,
7. nastavit bezpečný počáteční lifecycle.

AI sama nesmí neauditovaným importem vytvořit Puzzle jako `realized=A`.

---

# 50. AI provenance

Pokud je Puzzle nebo jeho část vytvořena AI, může být evidováno:

```text
origin=AI
provider
model
generation_timestamp
input_reference
```

Tyto údaje jsou auditní metadata a nesmějí být zaměňovány za schválení.

---

# 51. Priorita skutečnosti

Při konfliktu zdrojů platí:

1. skutečně ověřený stav prostředí,
2. kanonická SQLite evidence,
3. aktuální implementace a test evidence,
4. Git export odpovídající kanonickému stavu,
5. dokumentace,
6. historické informace.

Dokumentace sama nesmí přebít prokazatelně ověřený skutečný stav.

---

# 52. Žádné duplicitní mechanismy

## RFU-GOV-002

**MUST**

Před vytvořením nové utility musí být ověřeno, zda požadavek nelze implementovat rozšířením existující:

- Puzzle,
- Get,
- Git,
- Compare,
- AI,
- Feedback,
- Function registry služby.

RFUTIL 10 má preferovat kompozici společných funkcí před vznikem paralelních nástrojů.

---

# 53. Puzzle jako základ architektury RFUTIL 10

RFUTIL 10 vytváří tento hlavní vztah:

```text
                    RFUTIL 10
                        |
                     Puzzle
                        |
              Canonical SQLite
                        |
       +----------------+----------------+
       |                |                |
       v                v                v
   Functions           Git              AI
       |                |                |
       v                v                v
    Versions        Recovery          Handoff
       |
       v
     Tests
       |
       v
      FB
       |
       v
   Approval
       |
       v
   Manifest
       |
       v
 Build / Release
```

---

# 54. Cílový princip

RFUTIL 10 musí umožnit, aby:

- člověk mohl pokračovat v projektu,
- ChatGPT mohl pokračovat v projektu,
- Codex mohl pokračovat v projektu,
- Gemini mohl pokračovat v projektu,
- lokální AI model mohl pokračovat v projektu,
- projekt bylo možné rekonstruovat z Git reprezentace,
- SQLite bylo možné obnovit,
- stav implementace byl dohledatelný,
- manifest odpovídal skutečnosti,
- každá významná změna měla původ a historii.

RFUTIL se tím mění z kolekce utilit na **auditovatelný systém řízení implementace, znalostí, verzí a AI spolupráce**.

---

# 55. Minimální cílové CLI schopnosti

Finální syntaxe bude potvrzena implementačními Puzzle, ale RFUTIL 10 musí poskytovat schopnosti ekvivalentní:

```bash
rfu get src
rfu get src --commit

rfu get puzzle
rfu get puzzle --commit

rfu get AI --scope rfu
rfu get AI --scope module:<name>
rfu get AI --scope next
rfu get AI --scope puzzle:<id>

rfu get AI --auto
rfu get AI --compress
rfu get AI --auto --compress
rfu get AI --commit

rfu git status
rfu git version
rfu git compare src
rfu git compare puzzle

rfu puzzle import
rfu puzzle import --path <path>
```

Implementace může použít kratší kompatibilní aliasy, pokud zůstane zachován jednoznačný význam.

---

# 56. Implementace tohoto standardu jako Puzzle

Samotná realizace tohoto řídicího dokumentu a Puzzle architektury musí být rozdělena do řízených Puzzle.

Minimální oblasti:

```text
ARCHITECTURE
SQLITE_SCHEMA
PUZZLE_LIFECYCLE
FUNCTION_LINK
MANIFEST_LINK
FEEDBACK_SUMMARY
GET_ENGINE
GET_SRC
GET_PUZZLE
GET_AI
AI_AUTO
AI_COMPRESS
GIT_FOUNDATION
GIT_COMPARE
GIT_COMMIT
PUZZLE_EXPORT
PUZZLE_IMPORT
RECOVERY
DOCUMENTATION
TESTS
RELEASE_GATES
```

Rozdělení do konkrétních Puzzle ID provede kanonický Puzzle proces.

---

# 57. Release gate RFUTIL 10

První standardní release RFUTIL 10 nesmí být označen jako plně přijatý, dokud nejsou minimálně:

- vytvořené kanonické SQLite schéma Puzzle,
- implementovaný lifecycle,
- funkční export Puzzle,
- funkční import Puzzle,
- funkční `get src`,
- funkční `get puzzle`,
- funkční základ `get AI`,
- funkční Git resolver,
- funkční Git status/version,
- funkční compare SQLite Puzzle vs Git,
- ověřená vazba Puzzle → Function → Manifest,
- ověřené Feedback summary,
- vytvořená recovery procedura,
- dokončená základní dokumentace,
- provedené integrační testy.

---

# 58. Stav tohoto dokumentu

Aktuální stav:

```text
DOCUMENT_STATUS=DRAFT
ARCHITECTURE_VERSION=RFUTIL_10
IMPLEMENTATION_STATUS=NOT_STARTED
APPROVAL_STATUS=PENDING
```

Po uložení do kanonické RFUTIL dokumentace, vytvoření odpovídajícího Puzzle a schválení může být dokument povýšen na:

```text
DOCUMENT_STATUS=ACTIVE
```

---

# 59. Kanonické umístění

Doporučené umístění v RFUTIL:

```text
docs/
└── governance/
    └── RFU10_PUZZLE.md
```

Tento dokument musí být uveden v hlavním indexu projektové dokumentace jako:

```text
HIGHEST GOVERNING DOCUMENT – RFUTIL 10 Puzzle Architecture
```

---

# 60. Závěrečné pravidlo

## RFU-GOV-003

**MUST**

Žádná implementace RFUTIL 10 Puzzle, AI handoff, Git integrace, manifestového řízení nebo recovery nesmí obcházet principy tohoto dokumentu.

Pokud budoucí zkušenost ukáže, že některý princip musí být změněn, mění se nejprve tento řídicí standard verzovaným a auditovatelným postupem a teprve následně implementace.

---

**Konec dokumentu**

K RFU bych jej zařadil do hierarchie takto:

```text
docs/
├── INDEX.md
└── governance/
    ├── RFU10_PUZZLE.md      <- nejvyšší standard Puzzle/RFUTIL 10
    ├── RULES.md
    └── REGISTERS.md
```

A do `docs/INDEX.md` bych přidal tento dokument **úplně nahoru mezi závazné řídicí dokumenty**. Fyzický zápis do pracovního stromu RFU jsem teď neprovedl, protože přesun do režimu, který má přístup k projektu, byl odmítnut. Obsah výše je ale už připraven jako celý kanonický dokument, nikoli jen návrh osnovy.
