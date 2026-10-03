# RFU canonical ad-hoc evidence pipeline

## Purpose

Every ad-hoc diagnostic command used for RFU work should go through one canonical
evidence path instead of printing an untracked terminal-only result.

```text
command
  -> redacted local full.log
  -> compact AI_INFO.json
  -> RESULT.md (Shared Result Bus contract)
  -> Shared Result Bus projection when mounted
  -> existing bounded Storage sync for any controlled cloud mirror
  -> otherwise explicit PENDING handoff
```

Google Drive is never written by this wrapper. Existing Shared Result Bus policy
remains authoritative: automated GDrive persistence is prohibited and
`ACCEPTED` is never promoted automatically.

## Command

```bash
rfu evidence-run -- <command> [args...]
```

Optional stable IDs:

```bash
rfu evidence-run --wp-id WP-123 --cycle-id CYCLE-123 -- <command>
```

Emergency/local-only mode:

```bash
rfu evidence-run --no-shared-persist -- <command>
```

## Policy

For new RFU diagnostics, scripts, troubleshooting instructions and human-run
preflights, `rfu evidence-run` is the canonical wrapper. Direct shell execution
is a break-glass exception only when the wrapper itself is unavailable or under
repair. Such an exception must be followed by evidence capture before the result
is treated as project evidence.

Large logs stay local. Only compact AI/result evidence is eligible for automatic
Shared Result Bus persistence. Any cloud mirror is performed only by the existing
bounded Storage sync authority; this wrapper never creates a second cloud writer.

Every evidence result carries at least:
- run identity;
- source SHA plus provenance of how it was resolved;
- node/executor identity;
- technical result and RC;
- local-detail pointer to the redacted full log;
- Shared Result Bus handoff status.

## Evidence

Expected terminal footer:

```text
EVIDENCE_RUN_STATUS=OK|ERROR
RUN_ID=...
FULL_LOG=...
AI_INFO=...
RESULT=...
SHARED_RESULT_BUS_STATUS=OK|PENDING|...
GOOGLE_DRIVE_WRITES=NO
```

