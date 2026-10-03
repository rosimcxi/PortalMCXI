# Vhodné propojení PortalMCXI s RFU

## Začít jedním adaptérem

.NET RfuAdapter má poskytovat úzké operace: ověřit readiness, předat typovanou allowlisted práci, zjistit Run stav a načíst kompaktní výsledek. React nikdy nedostane RFU credential ani obecný shell endpoint.

Nejprve read-only status/evidence. Potom build/test a collectory; provozní změny až s příslušnými AC guardy. Transport vybrat podle skutečně dostupného RFU kontraktu na Contabo/HPA. Existence HTTP submit API zatím nebyla ověřena, takže se nevymýšlí konkrétní URL jako hotová služba.

Pokud vhodný veřejný HTTP kontrakt RFU chybí, použít existující intake/result adapter za interní hranicí, s mapováním na canonical RFU Queue. Nevytvářet druhou frontu v PostgreSQL Portalu. Případný HTTP bridge bude pouze adaptér existující autority.

## Request/result kontrakt – návrh

Požadavek nese project_id=PortalMCXI, request_id, Issue/Puzzle/work_id, SOURCE_SHA/BASE_SHA, EXPECTED_AUTHORITY, capability, trust, prioritu, idempotency key, resource estimate, policy_id a result destination. Tyto projektní položky je nutné kompatibilně namapovat na RFU kontrakt; nelze tvrdit, že dnešní dispatcher již všechny přijímá.

Výsledek nese RFU RUN_ID, request_id, source SHA, node/executor/model, status, RC, timestamps, artifact/checksum, kompaktní AI-info, detail-log pointer a handoff stav. Portal ukládá projekci pro zobrazení, nikoli vlastní autoritativní RFU stav.

Po timeoutu nejprve dohledat request/run podle idempotency key; nevytvářet duplicitní běh. Retry je stejná projektová spotřeba. Freshness ověřit při claim i těsně před mutací a publikací.

## Dva odlišné provozní toky

| Tok | Postup |
|---|---|
| Čtení dashboardu | React → .NET → uložená Portal data a omezená RFU status projekce |
| Práce na pozadí | oprávněný požadavek → RfuAdapter → RFU admission s 30% kvótou → Execution Hub → executor → evidence/result → normalizace do Portalu |

Collectory jsou Portal business funkce; RFU zajišťuje jejich plánování a provedení. Jeden scheduler, logický projektový účet, explicitní capability. Specializovanou SEN/VPN práci nesměrovat podle volného textu a neblokovat ji Portal úlohami.

## Bezpečnost a síť

RFU integrační rozhraní interní, autentizované, s allowlistem funkcí a minimálními scopes; deployment credential oddělený od dashboard read-only credential. Secret refs resolveuje existující bezpečná lokální autorita. Žádný credential v parametrech Queue/logu.

Portal veřejná UI/API za stávající proxy podle potvrzeného Contabo stavu. PostgreSQL a případný AI worker interní; nepublikovat DB port do internetu. Preferovat stejný origin pro `/api`, aby UI nemuselo mít natvrdo API doménu; finální NPM směrování ověřit před změnou.

## Důkazy před prvním běžícím propojením

1. Live RFU verze, zdrojové SHA, funkční kontrakt, identity a dostupný transport.
2. Vynucená project_id kvóta ≤30 %, atomické claimy a správná reakce na stale/unknown kapacitu.
3. Jeden bounded read-only Run, skutečná evidence od request až po výsledek a žádný secret leak.
4. Idempotentní retry a obnovení po timeoutu bez duplicit.
5. UI pravdivě rozlišuje CONFIGURED, READY, RUNNING, ERROR, STALE a UNKNOWN.

Převzaté JSON kontrakty jsou reference RFU, ne aktivní deployment konfigurace Portalu.
