# PortalMCXI – nejvýše 30 % zdrojů RFU

POLICY_ID=PORTALMCXI-RFU-RESOURCE-CAP
DECISION_SOURCE=Roman / 2026-10-03
MAX_SHARE=0.30
RUNTIME_ENFORCEMENT=PENDING
RFU_DISPATCH_WITHOUT_VERIFIED_ENFORCEMENT=DENY

## Význam

Všechny úlohy Portalu nesou project_id=PortalMCXI: vývoj, build, test, audit, collectory, AI, deploy i související retry. Sdílená práce se přičítá transparentně; rozdělení na jiné názvy úloh limit neobchází.

Strop platí pro skutečně použitelnou kapacitu RFU po rezervách OS, stávajících služeb, trust a capability kontrolách. Neznamená 30 % fyzického stroje ani 30 % počtu úloh bez ohledu na jejich váhu.

Konzervativní projektový požadavek na vynucení: maximálně 30 % přidělitelného CPU i RAM na použitých uzlech a v celém eligible poolu; GPU a provider concurrency/tokenové či finanční kvóty samostatně tam, kde se používají. U nedělitelného workeru/GPU nesmí zaokrouhlení zvýšit podíl nad strop. Práce čeká nebo se omezí; nedostane automaticky celý slot.

RFU URGENT/SEN a chráněné služby zachovávají prioritu. CHEAPEST_CAPABLE_FIRST platí až po kontrole identity, capability, trust, freshness a zbývajícího projektního rozpočtu. Offline/stale telemetry neznamená volnou kapacitu.

## Jediná autorita

Vynucení patří do existujícího Execution Hubu a jeho placement/lease/admission mechanismu. Portal nemá vlastní RFU scheduler ani registr uzlů. Policy JSON v tomto repozitáři je deklarace požadavku; RFU ji zatím nemusí umět načíst.

## Povinné kontroly

- Před claim a startem: aktuální kapacita, přidělené/running Portal práce, váha nové práce, zbývající budget a atomická lease.
- Před každou mutací a commit/push/apply: aktuální autorita a source freshness.
- Za běhu: měřit skutečnou spotřebu; omezit či checkpoint/requeue preempt-safe práci. Transakce se nesmějí přerušovat naslepo.
- Retry, child jobs, CI a přímý AI handoff používají stejný účet projektu.
- Nedostupný quota adapter či neprůkazná telemetrie: WAITING_CAPACITY/BLOCKED, žádný dispatch.

## Stav ověření

Prověřeno RFU main SHA a8f25138bc6999c1838c1b4f0e4d76dab03e3cdb, zejména execution_hub/capacity.py a worker_pool_policy_v1.json. Tyto soubory řeší uzlovou kapacitu a rezervy; projektový 30% limiter v nich nebyl nalezen. Nebyl proveden audit všech runtime cest ani nasazení. Proto není možné tvrdit, že limit je již v provozu vynucen.

V tomto kroku nebyl do RFU odeslán žádný Portal běh. Před prvním během je nutné doložit: souběžné claimy nepřekročí limit, stale stav dispatch blokuje, retry sdílí budget a skutečné runtime měření potvrzuje strop.
