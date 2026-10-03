# RFUTIL Conductor Shared Result Bus

STATUS=ACTIVE
CANONICAL_ROOT=/RFU/CONDUCTOR_SHARED
GOOGLE_DRIVE_WRITE=PROHIBITED_AUTOMATION
SHARED_LIBRARY_WRITE=ALLOWED_AUTOMATION
GOOGLE_DRIVE_PERSISTENCE=MANUAL_ONLY_ON_EXPLICIT_ROMAN_REQUEST
ACCEPTED_AUTO=NO

## Contract
- Main result: `results/<RUN_ID>.md` plus `state/LATEST_MAIN_RESULT.md`.
- Planner: `plans/<PLAN_ID>.md` plus `state/LATEST_PLAN.md`.
- Collector correlates `CYCLE_ID/RUN_ID/WP_ID`, writes `cycles/<CYCLE_ID>.md` plus `state/LATEST_CYCLE_RESULT.md`.
- Persistence gate never writes Google Drive. If manual persistence is needed it only updates `state/MANUAL_PERSIST_PENDING.md`.
- Stable idempotency keys: RUN_ID, PLAN_ID, CYCLE_ID, PERSISTENCE_KEY.
- Historical plan/result/cycle files are append-by-new-ID; pointer files are overwrite-in-place.
- Scheduler execution is not result evidence. `HOTOVO_BY_RESULT` requires a real non-empty result.
- `ACCEPTED` remains Roman-only.

