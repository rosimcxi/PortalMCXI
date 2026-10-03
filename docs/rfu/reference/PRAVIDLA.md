# RFUTIL 10 – stručná závazná pravidla

**Version:** 10.0.31  
**Updated:** 2026-09-25

1. SQLite je kanonická evidence; web, CLI a AI export nesmí mít paralelní zdroj pravdy.
2. Puzzle = vývoj/změna/řešení; Function = spustitelná schopnost; Snippet = znovupoužitelný vzor/kód; Playbook = opakovaný postup; Task/Úkol = reálná provozní práce.
3. Každý použitý Snippet musí mít dohledatelnou vazbu `used-by`; změna Snippetu musí umožnit impact analýzu.
4. Historický chybný kód se neportuje; zachovává se evidence, dobrý vzor/test a `must_not_regress`.
5. Read-only/plan je výchozí. Skutečná změna vyžaduje explicitní apply/write a podle kontraktu `--AC`; samotná generace artefaktů `--AC` nepotřebuje.
6. Žádný plaintext secret/master key do Gitu, logu ani AI exportu.
7. Status a Severity jsou oddělené; každý důkaz má timestamp/RUN_ID/verzi/SHA podle významu.
8. Build vzniká až podle schváleného Build Assignmentu a po vydání je immutable.
9. `TESTED_TARGET` a `ACCEPTED` se nikdy nepředpokládají.

> **Nejvyšší lidsky čitelná RFU autorita:** [RFU_RIDICI_PREHLED.md](RFU_RIDICI_PREHLED.md) — Řídící přehled RFU. Obsahuje kanonické pojmy, aliasy, vstupy/výstupy, 7 balíků, Unit model, konfiguraci, cesty, vývoj, testování, BLOCK pravidla, architektonické diagramy a Conflict Review. Při rozporu má přednost schválený význam z Řídícího přehledu; detailní/historické dokumenty jsou podpůrné evidence.

## APPROVED — HPA normal queue via Execution Hub — 2026-09-25
- Autorita placementu zůstává **Execution Hub #207** nad live node truth z **Identity #272**; nevzniká druhý HPA scheduler/fronta.
- Dočasná rezervace **#284** skončila 2026-09-24 23:59:59 Europe/Prague; HPA je nyní `NORMAL`.
- Před každým novým HPA handoffem se kontroluje fresh availability/capability/trust/capacity a reserved práce v pořadí `URGENT > SEN > standard placement`.
- `URGENT` je explicitní P0/P1 urgent policy/tag/marker; `SEN` pouze explicitní SEN identita/domain/capability/work-package. Neodhadovat z volného textu.
- Při prázdné reserved frontě může HPA vzít jednu normální eligible úlohu; potom `RECHECK_REQUIRED`.
- PREEMPT_SAFE normální práce: reserved recheck cca 60 s; >5 min remaining => checkpoint/requeue/switch, ≤5 min => doběhnout a okamžitě recheck.
- DB write/migrace/transakce a jiné non-preempt-safe práce se automaticky nepřerušují.
- Generic Linux/build/CI zůstává při stejné capability preferované na Contabo; HPA pro corporate/VPN/SEN/INX/PG/`db.readonly-target` podle capability.

## APPROVED — RFU 11.1.x incremental distribution — 2026-09-25
- Průběžná RFU řada používá jednu globální verzi `11.1.N`; každá distribuovaná změna po build/test PASS zvýší `N` o 1.
- Stejná distribuce má stejnou `RFU_VERSION` na HPA, Contabo, MacMax/M1, M2 a dalších uzlech; node-specific metadata nesmí měnit RFU verzi.
- Každá distribuce je immutable a deklaruje přesně `REQUIRES_PREVIOUS=11.1.(N-1)`.
- Distribuci `N` nelze přímo aplikovat na jinou verzi než `N-1`; uzel více verzí pozadu musí projít sekvenčním chainem až na latest.
- `rfu upgrade` získá z PRIMARY GitHub autority nejvyšší dostupnou verzi a aplikuje všechny chybějící distribuce v pořadí bez přeskakování.
- `rfu ver`, Welcome a Web vždy ukazují skutečně nainstalovanou verzi, nikoli source HEAD/candidate/plánovanou verzi.
- Po PASS se distribuce rolluje primárně na HPA a následně na další dostupné policy-allowed uzly; nedostupný uzel vydání neblokuje.
- Candidate/test označení se nepoužívá jako uživatelský lifecycle stav distribuce. Test evidence zůstává, ale nainstalovaná distribuce je `RUNNABLE`.
- Chybějící `TESTED_TARGET` nebo `ACCEPTED` samo o sobě neblokuje spuštění platné nainstalované funkce. Skutečný `BROKEN/UNUSABLE`, safety/policy zákaz, chybějící dependency/secret/target nebo version-chain mismatch blokovat může.
- Web používá stejný okamžitý incremental rollout jako ostatní RFU části a nečeká na kompletní balík.
- Kompletní fresh-install ZIP/package se vytváří pouze na explicitní požadavek uživatele; běžné existující instalace používají malé incremental distributions.
- Autorita: `RFU_RIDICI_PREHLED.md`, sekce „Vývoj, verze, distribuce a testování“.

## APPROVED — Communication governance — 2026-09-25
- Sekce **„Řízení komunikace a pokračování práce“** v `RFU_RIDICI_PREHLED.md` je závazná pro **výstupy určené člověku**: chat, console/CLI, Web/UI a obdobné prezentační vrstvy.
- Lidské volby se **nevkládají automaticky do interních AI→AI promptů**.
- AI→AI handoff používá co nejmenší strojový kontext, zejména `RUN_ID | PARENT_RUN_ID | STATUS | RC | BLOCKERS | NEXT_ACTION | SOURCE_SHA | ARTIFACT/EVIDENCE | DEPENDENCIES`.
- Lidský pracovní výstup končí stručným návrhem dalšího postupu.
- Více smysluplných cest se čísluje; volby se přizpůsobují situaci a nejsou pevná šablona.
- Pokud je to vhodné, jedna volba je finální/uzavírací.
- Pokud existuje jeden bezpečný zřejmý krok, nevyrábějí se umělé varianty.
- Rozhodnutí vyžadující člověka se oddělují od práce, která může bezpečně pokračovat.
- Při postupném zpřesňování se zachovává dříve schválený obsah, pokud není výslovně změněn.
- Toto není nový paralelní zdroj pravdy; autoritou zůstává `RFU_RIDICI_PREHLED.md`.


## RFU Git / OBJECTS path authority — 2026-09-16
- Jediný aktivní RFU Git workspace na Windows je `C:\git\rfu`.
- Jediný aktivní WSL pohled na tentýž workspace je `/mnt/c/git/rfu`.
- Aktivní funkce nesmějí vlastnit fyzickou Git/Object cestu; používají centrální `path.git` / `path.objects` přes Core Service/Identity config a Storage Router.
- Git-visible casing objektového stromu je `OBJECTS`, protože zachovaná DBGit Git historie používá `OBJECTS/...`.
- Na současném Windows-backed workspace jsou `C:\git\rfu\OBJECTS` a `C:\git\rfu\objects` stejný fyzický adresář; case-only přepis se nesmí dělat bez explicitní migrace.
- Jediný kanonický remote repository je `rosimcxi/rfu`.
- `C:\git\rfu` a `/mnt/c/git/rfu` jsou dvě cesty k témuž fyzickému working tree, nikoli dvě repo autority.
- Jakékoli RFU/sen-db repo nebo OBJECTS pod `D:\...`, `/mnt/d/...`, `_Cloud\GIT\rfu`, `sen-db`, `/home/fiser/src/RFU-Linux` nebo jiným paralelním rootem je `INVALID_FOR_ACTIVE_USE / HISTORICAL_ONLY`.
- `\\wsl.localhost\AlmaLinux-9\home\fiser\src\RFU-Linux` je pouze Windows pohled na stejnou zneplatněnou WSL cestu.
- Historické soubory na starých cestách se smějí pouze read-only dohledat/reconciliovat; nesmí být runtime fallback, CURRENT source, publish target ani Git autorita.
- Aktivní kód musí failnout/regresně selhat, pokud RFU Git nebo OBJECTS resolve skončí na staré D:/sen-db/home-fiser-src cestě.
- Podrobná autorita: `docs/RFU_GIT_PATH_AUTHORITY.md`.

## RFUTIL 10.0.15 - Self-Heal safety rule
- Self-Heal MUST nejdrive klasifikovat finding a ulozit auditni evidence.
- Automaticky lze provest pouze lokalni/syntaktickou nebo jednoznacne reverzibilni opravu bez databazove zmeny (napr. invalidni runtime path s jednoznacnym fyzickym modulem).
- Sémanticka nebo destruktivni DB oprava MUST skoncit u immutable planu; apply vyzaduje explicitni approval/--AC a nasledny retest.
- Self-Heal nesmi maskovat root cause: fallback vraci WARNING a zaklada/navazuje realny ukol na trvalou opravu.



## RFUTIL 10.0.18 — Integrity, governance visibility and selected/free parameters
- `DOK/RIDICI/PUZZLE/RFU_Puzzle_Conductor_v6.docx` is the current Conductor reference, version 6.0, status NÁVRH; WebUI must surface the version/status and must not silently treat it as approved implementation.
- Selected/free parameters are resolved only from the central System Parameter Registry. The currently registered SEN DB group is `senall`; UI/CLI must enumerate registry groups instead of relying on remembered names.
- Legacy `--database ALL` is an explicit registry alias to the selected DB group; literal `ALL` must never reach pg_dump/Informix/database tools through that legacy option.
- Unknown selected names such as an unregistered `sendb` fail closed; self-heal must not guess semantic aliases.
- Self-heal may repair only unambiguous technical/syntactic inconsistencies. Database-changing repair remains behind explicit plan + `--AC` + retest.


## RFUTIL 10.0.19 — DBGit snapshot refresh contract
- `rfu dbgit load-inx` and `rfu dbgit load-pg` MUST always read the live source and create a new immutable snapshot on success. Existing CURRENT is history, never a cache/skip condition.
- The prior CURRENT MUST remain auditably historical; successful new snapshot becomes CURRENT.
- `compare-snapshots` without `--force` compares existing CURRENT snapshots and MUST NOT reread live databases.
- `compare-snapshots --force` MUST refresh both left and right live sources first and compare only database pairs where both sides advanced to new snapshot IDs.
- Forced compare MUST NOT silently fall back to older snapshots after a refresh failure. Failed pair is `COMPARE=SKIPPED` with `REASON=FORCED_REFRESH_FAILED`; other safe pairs continue.
- Compare output MUST expose exact left/right snapshot IDs and capture timestamps.

## RFUTIL 10.0.20 – povinný Function Contract a Artifact Path
- Každá funkce MUSÍ mít z CLI i WebUI dohledatelný aktuální popis funkčnosti, podmínky/výjimky, zadání/zdroj a popis testování.
- CLI a WebUI MUSÍ používat stejný kanonický `function_contracts` v System SQLite; Web nesmí udržovat paralelní popis.
- Každý GIT/Object snapshot, diff/compare/analyze a backup výstup MUSÍ při vzniku registrovat `artifact_id`, `physical_path` a `directory_path` v `artifact_registry`.
- Historická cesta se uchovává i po vzniku novějšího běhu. Pokud fyzický adresář později zmizí, evidence se nemaže; UI zobrazí `Existuje=NO`.
- Web smí procházet filesystem pouze přes registrované `artifact_id`; libovolný `path=` vstup je zakázán.


## RFUTIL 10.0.23 – release hardening invariants
- Runtime validační test nesmí natvrdo očekávat historickou Module-Version, pokud kontrakt požaduje efektivní/aktivní roli; výjimka musí být explicitně označený fixture test.
- Web `HTTP 200` není funkční důkaz. Release gate musí ověřit povinné obsahové a navigační invarianty.
- Artifact ID musí být deterministické pro stejný run/function/module/type/path. Registrační timestamp nesmí měnit identitu.
- Windows/WSL `/mnt/<drive>` cesty se pro identitu porovnávají case-insensitive; nativní Linux cesty zůstávají case-sensitive.
- Jednoznačně shodné historické artifact duplicity lze přes `repair --AC` sloučit pouze se zachováním starých ID jako aliasů. Stejný producent může v novém runu obnovit CURRENT/GIT cestu; konflikt stejné cesty s jiným producentem nebo artifact type se nesmí automaticky opravovat.
- Additive SQLite migrace musí být testována na legacy schématu a musí vytvořit sloupce před indexy/constraints, které je používají.
- Read-only SQLite operace musí explicitně zavřít connection; warning-enabled regresní běh nesmí hlásit ResourceWarning.
- Exact-ZIP fresh install + runtime hash + Auto Health + `rfut all` je povinný před lokálním release claimem.


## RFUTIL 10.0.24 – compatibility and immutable-build guard
- Jakmile fyzicky vznikne ZIP/package ID, jeho obsah ani význam se nesmí přepsat. Zjištěná packaging/runtime regrese vyžaduje nový vyšší System version i package ID.
- `10.0.23 / 001023` je auditně `REJECTED_LOCAL_BUILD`; nesmí být znovu publikován jako opravený balík.
- Canonical artifact path má sémantického vlastníka `(function_id, module_id, artifact_type)`. Stejný vlastník smí v novém runu obnovit CURRENT/GIT cestu a dostane nový artifact event/ID.
- Jiný vlastník nebo jiný artifact type na stejné canonical path je `ARTIFACT_PATH_OWNER_CONFLICT` a nesmí se automaticky opravovat.
- Release gate musí testovat i kompatibilitu nezměněných doménových modulů proti změněné System vrstvě; úspěch System unit testů sám nestačí.
- System version a schema version se nesmí násilně sjednocovat. Auto Health musí deklarovat expected/actual schema a zda je migrace potřeba.


## RFUTIL 10.0.25 – subprocess resource lifecycle gate
- `subprocess.Popen(..., stderr=PIPE)` musí child vždy ukončit/reapnout a PIPE explicitně zavřít; samotné `wait()` není dostatečný resource-lifecycle kontrakt.
- Release regression test musí po Web probe vynutit garbage collection a selhat při jakémkoli `ResourceWarning`.
- Post-build warning z exact ZIP je release finding: existující package se nepřepisuje, ale zamítne a oprava dostane nové version/package ID.


## RFUTIL 10.0.26 – Web runtime a skutečné linky
- `HTTP 200` ani úspěch izolovaného testovacího Web procesu není důkazem, že uživatelův běžící Web odpovídá nainstalované verzi.
- Každý běžící Web musí zveřejnit runtime identity (version/package/HOME/PID/start/browser SHA-256) a Auto Health ji porovnává s instalovaným kódem.
- Stale RFUTIL Web je `BAD`; bezpečný explicitní zásah je `rfu web restart --AC`. Cizí proces na portu se automaticky nezabíjí.
- Function, Module, Backup a Artifact seznam musí mít samostatné action linky. Test nesmí uznat pouhou přítomnost textu nebo obecného href jako plný UI kontrakt.
- Artifact soubor se otevírá pouze přes registrované `artifact_id` a relativní cestu uvnitř povoleného rootu; traversal, absolutní cesta a symlink escape jsou zakázány.


## RFUTIL 10.0.28 – dokumentace a background Web actions
- CZ je výchozí uživatelský jazyk; EN je technická/AI varianta.
- Maintained CZ/EN dokumenty mají stejné Document-ID, Version a sekční kontrakty.
- Bohatá historická dokumentace se nesmí zkrátit bez explicitního důvodu a evidence.
- Dlouhá Web akce nesmí být vlastněna HTTP requestem; životní cyklus vlastní persistentní RFUTIL RUN/JOB.
- Po enqueue se okamžitě zobrazuje bezpečný kopírovatelný CLI příkaz a RUN_ID.
- Secrets se do Run/Job/command evidence nikdy nepersistují.
- `--AC` musí být vyřešeno před enqueue; worker nečeká na interaktivní potvrzení.
- Recovery nikdy neodvozuje COMPLETED pouze z neexistence PID.
- Běhy, procenta, výsledky, Artifact, Feedback a Error Lineage jsou společné pro Web a CLI.


## RFUTIL 10.0.30 - exact-ZIP usability regression hardening
- 001028 se nepřepisuje; zůstává REJECTED_LOCAL_BUILD.
- `rfu function` a `rfu modules` jsou povinné end-to-end CLI release gaty.
- Aktuální System verze se vybírá semanticky, ne lexikálně.
- Česká Function/Module nápověda nesmí zobrazovat syrový anglický technický kontrakt; EN technická vrstva zůstává zachována.
- Schema 10.0.28 zůstává pro System 10.0.30 beze změny záměrně.


## RFUTIL 10.0.31
- Baseline: 10.0.30 / 001030.
- DBGit 9.171 adds tested pg_dump binary selection.
- All current chat-derived WP definitions are carried as provisional evidence.
- HPI7 fresh-install templates are versioned in System payload.
- TESTED_TARGET=NO; ACCEPTED=NO.

## APPROVED — RFU DB source / target / remote authority — 2026-09-21

**Approval:** schváleno uživatelem v chatu dne 2026-09-21.  
**Applies from:** RFU Functions 0.1.1 / DB Core Recovery.  
**Work authority:** #252, parent #146, DB-core #217, target authority #198, authority repair #237.

### A. Database / scope — one selector
- Web/CLI UX uses one canonical `Database / scope` selector. The former simultaneous `Free DB (--db)` + `Selected DB/group (--s_db)` controls are not presented as competing inputs.
- Default selection is `senall`.
- `senall = eg, ns, poslanci, pssenat, publsenat`.
- Backend emits exactly one appropriate scope form: explicit DB -> `--db <db>`; registered group -> `--s_db <group>`. It must never generate both for the same selection.
- EXT eligibility is target-specific: `eg` is ineligible on EXT and must be reported as `SKIP_POLICY/INELIGIBLE` without stopping independent DBs.
- Unknown DB/group names fail closed; semantic aliases are never guessed.

### B. PostgreSQL source / targets
- Object/GIT PostgreSQL uses one registry-driven selector `PG source / targets`.
- Default is `ALL_REMOTE`.
- `ALL_REMOTE` means all currently registered, capability-eligible and policy-allowed remote SEN PostgreSQL targets. It is a registry/group identity, not a hard-coded list.
- Canonical PostgreSQL target identities are:
  - `DEV_INT`
  - `DEV_EXT`
  - `ACC_INT`
  - `ACC_EXT`
  - `TEST`
  - `PROD_INT`
  - `PROD_EXT`
- Registration does not imply readiness. A target without configuration/readiness evidence is shown truthfully as `NOT_READY/CONFIG_REQUIRED` and does not block independent eligible targets.
- ACC compare/check/audit operations remain read-only unless a separate explicitly approved write operation exists.

### C. Informix source / targets
- Canonical Informix target identities are:
  - `INX_ACC_INT`
  - `INX_ACC_EXT`
  - `INX_PROD_INT`
  - `INX_PROD_EXT`
- `INX_INT` is compatibility alias only -> `INX_ACC_INT`; it is never a second endpoint/profile/configuration authority.
- `INX_ACC_EXT + eg` is rejected before connection as `INELIGIBLE/SKIP_POLICY`.
- Credentials/endpoints are resolved only through central configuration + secret references; literal secrets/endpoints must not enter Git, result evidence or logs.

### D. Git destination / remote — separate axis
- Database source/target and Git publication destination are separate dimensions.
- UI/contract uses a registry-driven `Git destination / remote` selector.
- Default is `ALL_CONFIGURED_REMOTES`.
- Remote providers supported by the registry must include at least:
  - GitHub
  - GitLab
  - Bitbucket
  - Azure DevOps
- Every remote configuration exposes at least:
  `REMOTE_ID | PROVIDER | REPOSITORY | BRANCH | OBJECTS_PATH | ROLE | READ_ALLOWED | WRITE_ALLOWED | PUBLISH_ENABLED | READINESS | SECRET_REF`.
- Secrets/tokens are never stored directly in remote configuration.

### E. Primary Git authority and mirrors
- PRIMARY Git authority remains:
  - provider: `GitHub`
  - repository: `rosimcxi/rfu`
  - branch: `main`
  - object tree: `OBJECTS/`
- Additional GitLab/Bitbucket/Azure DevOps remotes may be configured as `MIRROR | BACKUP | BUILD | DR`; they do not become competing source-of-truth authorities merely because they receive a copy.
- `ALL_CONFIGURED_REMOTES` publishes only to remotes whose policy/readiness permits the operation and records each child result independently.
- PRIMARY authority changes require a new explicit approved decision.

### F. Canonical Git and output paths
- Active Git workspace:
  - Windows: `C:\git\rfu`
  - WSL: `/mnt/c/git/rfu`
  - logical keys: `path.git`, `path.objects`
- Active OBJECTS root:
  - Windows: `C:\git\rfu\OBJECTS`
  - WSL: `/mnt/c/git/rfu/OBJECTS`
  - Git-visible casing: `OBJECTS`
- Canonical new-write artifact/output root is resolved centrally:
  - Windows: `C:\RFU_export`
  - WSL: `/mnt/c/RFU_export`
- Modules must use Identity/Core Storage/Storage Router and must not own hard-coded physical destinations.

### G. Forbidden historical destinations for new writes
The following are `HISTORICAL_ONLY` and may be read only for reconciliation/history. They must never be a runtime fallback, CURRENT source, active Git authority, publish target or new artifact destination:
- `D:\_Cloud\GIT\rfu`
- `/mnt/d/_Cloud/GIT/rfu`
- `D:\_Cloud\GIT\sen-db`
- `/mnt/d/_Cloud/GIT/sen-db`
- any `sen-db/OBJECTS` or `sen-db/objects`
- `/home/fiser/src/RFU-Linux` and its Windows UNC view
- `D:\RFU_EXPORT` / `/mnt/d/RFU_EXPORT` for new writes
- `/tmp` as a final durable destination

### H. Required run/result provenance and outputs
Every parent and child DB-core run must be independently identifiable. The result contract must persist, as applicable:

`RUN_ID | PARENT_RUN_ID | FUNCTION_ID | FUNCTION_VERSION | MODULE_ID | MODULE_VERSION | SOURCE_REPO | SOURCE_BRANCH_REF | SOURCE_COMMIT_SHA | SOURCE_TYPE | SOURCE_TARGET | DESTINATION_TARGET | DATABASE | SCOPE | RESOLVED_TARGET_ID | GIT_REMOTE_ID | GIT_PROVIDER | GIT_REPOSITORY | GIT_BRANCH | OBJECTS_PATH | PATH_LOGICAL_KEY | PHYSICAL_OUTPUT_PATH | STARTED_AT | FINISHED_AT | EXECUTION_STATUS | DIAGNOSTIC_VERDICT | RESULT_ARTIFACT_STATUS | RC | ARTIFACT_ID | SHA256 | SNAPSHOT_ID | OBJECT_COUNT | COMMIT_SHA | REMOTE_READBACK | FINDINGS | BLOCKERS | NEXT_ACTION`

Required capability outputs:
- **Object/GIT Load:** exact source target+DB, snapshot id, object count, SQLite/artifact identity, Git destination(s), commit/readback status, CURRENT/history truth.
- **Object Compare:** exact left/right snapshot ids and capture times, per-object SAME/DIFFERENT/LEFT_ONLY/RIGHT_ONLY, full definitions/diff artifact.
- **INX->PG Data Compare:** per-table counts and statuses, complete detail artifact, bounded console detail; NULL-vs-empty INFO, right-only INFO, LOB/BYTEA INFO/hash/metadata according to policy.
- **DB Check/Audit:** connection/access, ownership, grants/roles, sequences/identity/serial, tables/columns/types, PK/UNIQUE/FK, indexes, triggers, routines/signatures/dependencies, FDW, replication, WAL/space, config/path authority, snapshot consistency; each finding includes cause/evidence/next_action.
- **Backup:** target/db/scope, artifact path/id, size, SHA256, start/end, RC/status, readback/existence; restore validation only on approved safe TEST/sandbox target.
- `EXECUTION_STATUS`, `DIAGNOSTIC_VERDICT` and `RESULT_ARTIFACT_STATUS` are separate. `COMPLETED != OK`.

### I. Manual testing versus verification/release
- `FAILED_TEST` alone does not block a manual safe test execution of an exact Function-Version that is `ACTIVE + USABLE + role assigned` and policy-allowed.
- `FAILED_TEST` continues to block release/promotion claims.
- `UNUSABLE/BROKEN`, policy denial, missing required target mapping, or a destructive confirmation/AC gate remain blocking.
- A module-level self-test failure must not blindly stamp every function in the module as a proven capability failure.
- Timeout after capability-level `STATUS=OK` is classified separately from a proven functional failure.
- `ACCEPTED` remains human-only and `TESTED_TARGET` requires real target evidence.

### J. Approved-decision conflict handling
- Newer approved decisions are the default precedence signal, but **date alone never silently resolves a semantic conflict**.
- Whenever RFU, Process Coach, AI, documentation reconciliation or implementation detects two approved/current instructions that conflict, overlap ambiguously or are only conditionally compatible, it must surface a **Conflict Review** before treating the conflict as settled.
- Every Conflict Review must show at least:
  `CONFLICT_ID | AREA | OLDER_RULE + DATE | NEWER_RULE + DATE | EXACT_CONFLICT | AFFECTED_SCOPE | WHERE_OLDER_STILL_VALID | WHERE_NEWER_VALID | RISKS | PROPOSED_RESOLUTION | REQUIRED_CHANGES | DECISION_REQUIRED`.
- The proposal must state whether the correct resolution is:
  - `NEWER_SUPERSEDES_OLDER_FULLY`
  - `NEWER_SUPERSEDES_OLDER_PARTIALLY`
  - `BOTH_VALID_BY_SCOPE`
  - `OLDER_RETAINED_EXCEPTION`
  - `MERGED_CANONICAL_RULE`
  - `UNRESOLVED_BLOCKING`
- No contradictory approved rule may be silently deleted or reinterpreted. The older text remains auditable and is marked with its final relationship once the human decision is approved.
- The **approved final meaning of the conflict**, including scope and exceptions, becomes the new canonical rule and supersedes informal interpretations.
- Until a material conflict is approved, fail closed only for the conflicting scope; unrelated work continues.
- Process Coach/AI should surface the conflict together with a concrete recommended resolution, not merely report that a mismatch exists.

### K. Decision precedence
- If no semantic conflict exists, newer approved decisions supersede older incompatible defaults.
- If a semantic conflict exists, section J applies: explicit Conflict Review + human approval determines the final canonical meaning.
- Historical evidence remains immutable and is never rewritten to pretend that the newer rule existed at the time.

