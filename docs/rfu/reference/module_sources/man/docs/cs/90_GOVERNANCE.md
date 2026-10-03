<!--
Document-ID: RFU-MAN-90-GOVERNANCE
Document-Version: 1.3.0
Language: CZ
Counterpart: ../en/90_GOVERNANCE.md
Historical-Source: docs/history/1.2.0/cs/90_GOVERNANCE.md
-->

# Governance a bezpečnost

## Immutable release

Vydaný/reálně použitý package se nepřepisuje. Oprava dostává novou vyšší verzi.

## Function role vs verze

Role je samostatná evidence nad Function-Version. Module package může nést stejnou Function-Version v novější opravné package verzi.

## Změny

- read-only je výchozí;
- DB změny vyžadují explicitní `--AC` tam, kde je guard podporován;
- secret redaction je povinná;
- chyba jednoho objektu/DB nemá zastavit ostatní bezpečně pokračovatelné kroky;
- test evidence musí pravdivě rozlišovat `TESTED_LOCAL` a `TESTED_TARGET`.

## Dokumentace

Welcome je krátký rozcestník. MAN je praktická provozní dokumentace. Hlubší řídicí/historické informace zůstávají v System docs/governance a jsou dostupné přes Web `MAN/Docs`.


## Dokumentační governance CZ/EN

Každý nový významný dokument má CZ uživatelskou a EN technickou/AI variantu se stejným `Document-ID` a `Document-Version`. Release gate kontroluje existenci páru, povinné sekce a minimální obsahovou paritu. Historický bohatý dokument se nesmí tiše nahradit několika řádky.

## Background execution governance

Browser není process owner. Dlouhé operace vlastní persistentní RFUTIL RUN. Secrets se neukládají do Job parametrů, destruktivní `--AC` se potvrzuje před enqueue, double-submit je idempotentní a souběh řídí explicitní Function execution policy/lock scope.

