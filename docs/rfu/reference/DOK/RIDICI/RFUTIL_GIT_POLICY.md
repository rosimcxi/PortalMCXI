# RFUTIL Git source policy

Status: ACTIVE
Date: 2026-09-01

## Git contains
- complete text/source tree
- documentation, including `.docx`
- Puzzle items under stable Puzzle IDs
- Work Packages under stable WP IDs
- source-controlled manifests and checksums that match the Git tree

## Git does not contain
- `*.zip`
- `*.sqlite`
- install/runtime databases
- transport package wrappers named by date/version

## Puzzle layout
Do not create version/date export folders under `puzzle/`.

Canonical examples:

```text
puzzle/
  PUZZLE-000123/
    puzzle.json
    requirement.md
    solution.md
    tests.json

  work_packages/
    AIQ-001.md
    DBGIT-P0-06.md
    RFU-RELEASE-FINAL-GATE-001.md
```

The stable Puzzle/WP ID is the path identity. Git commits provide version history.

A transport ZIP may exist outside Git. On import its contents are merged into the stable Puzzle/WP paths; the ZIP wrapper itself is not committed.

## Installation artifacts
SQLite databases and ZIP packages remain in installation/artifact storage and may be included in installation/COMPLETE packages, but not in the source Git repository.

## Acceptance
Git commits and automated processing never set `ACCEPTED`. Acceptance remains Roman-only.

