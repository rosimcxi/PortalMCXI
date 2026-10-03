# RFU Git / OBJECTS path authority

Effective: 2026-09-16

## Single active authority

| Purpose | Windows | WSL/Linux view |
|---|---|---|
| RFU Git working tree | `C:\git\rfu` | `/mnt/c/git/rfu` |
| RFU OBJECTS tree | `C:\git\rfu\OBJECTS` | `/mnt/c/git/rfu/OBJECTS` |
| Canonical remote repository | `rosimcxi/rfu` | `rosimcxi/rfu` |

`C:\git\rfu` and `/mnt/c/git/rfu` are two views of the **same physical working tree**, not separate repositories.

On the current Windows-backed workspace, `OBJECTS` and `objects` resolve to the same physical NTFS directory (same device/inode through WSL). Git history preserved in `rescue/dbgit-snapshots-20260910-11` records the tree as `OBJECTS/`. Therefore canonical Git casing is `OBJECTS` to preserve history and avoid a case-only rewrite. Consumer code must still use the logical `path.objects` alias rather than hardcoding the physical name.

## Invalid for active use

The following are **INVALID_FOR_ACTIVE_USE / HISTORICAL_ONLY** as RFU Git repository roots, DBGit publish roots, OBJECTS roots, CURRENT sources, runtime fallbacks or source authorities:

- `D:\_Cloud\GIT\rfu`
- `/mnt/d/_Cloud/GIT/rfu`
- `D:\_Cloud\GIT\sen-db`
- `/mnt/d/_Cloud/GIT/sen-db`
- any `sen-db\OBJECTS`, `sen-db/OBJECTS`, `sen-db\objects` or `sen-db/objects`
- `/home/fiser/src/RFU-Linux` and its Windows UNC view `\\wsl.localhost\AlmaLinux-9\home\fiser\src\RFU-Linux`
- any other RFU clone/worktree used as an implicit canonical repository
- any OBJECTS directory outside canonical `C:\git\rfu\OBJECTS` when used as current/publish authority

Historical files on those locations are retained only as read-only provenance/evidence for reconciliation. They must never be silently selected by active code.

## Required runtime behavior

1. All active RFU source Git operations resolve through the central path/config authority; consumers must not own physical repo literals.
2. DBGit/Object definitions and Git-suitable reports publish only under canonical Git tree `OBJECTS/` in that same working tree.
3. Canonical logical key is `path.objects`, derived from `path.git`; current physical value is `C:\git\rfu\OBJECTS` / `/mnt/c/git/rfu/OBJECTS`.
4. No module may hardcode D:, sen-db, `/home/fiser/src/RFU-Linux`, or any other parallel repository as a default/fallback.
5. A locally created object artifact is not considered Git-published until run/artifact metadata records publication state and Git identity.
6. Artifact discovery must resolve from run/artifact registry/storage routing to the canonical physical path.
7. Historical immutable module/source versions may contain old path text as provenance only.
8. Regression/path gates must distinguish active defaults/fallbacks from historical/test evidence and fail only active path drift.
9. A case-only `objects`/`OBJECTS` rewrite is forbidden unless explicit migration evidence requires it; on the current workspace both names are one physical directory, while preserved Git history uses `OBJECTS`.

## Canonical OBJECTS contract

Historical/current Git examples:

- `C:\git\rfu\OBJECTS\DEV_INT\eg\CURRENT.json`
- `/mnt/c/git/rfu/OBJECTS/DEV_INT/pssenat/...`
- future versioned layouts may include `OBJECTS/definitions/...` and `OBJECTS/reports/...` behind `path.objects`.

The exact internal layout may evolve through versioned RFU contracts, but the Git-visible root casing remains `OBJECTS` unless an explicit migration is approved.

## Governance

`PATH_AUTHORITY=RFU_IDENTITY_CONFIG`
`GIT_REPOSITORY=rosimcxi/rfu`
`WINDOWS_GIT_ROOT=C:\git\rfu`
`WSL_GIT_ROOT=/mnt/c/git/rfu`
`OBJECTS_ROOT=C:\git\rfu\OBJECTS`
`OBJECTS_DIRNAME=OBJECTS`
`OBJECTS_CASE_SOURCE=PRESERVED_GIT_HISTORY`
`LEGACY_D_DRIVE_REPOS=HISTORICAL_ONLY`
`LEGACY_WSL_SRC_REPO=HISTORICAL_ONLY`
`SEN_DB_REPO=HISTORICAL_ONLY`

