#!/usr/bin/env bash
# NAME=PORTALMCXI_EVIDENCE_HANDOFF
# DATE=2026-10-07
# VERSION=1.0.0
# PURPOSE=Overi RFU self-description a vytvori cloudovy prehled blokaci Portalu.
# USAGE=bash scripts/portal_evidence_handoff.sh na HPA s dostupnym RFU.
# SAFETY=Zadny Portal job, SSH audit, DB pristup, deploy, merge nebo model call.
set -Eeuo pipefail
umask 077

if [[ "${1:-}" != "--evidence-child" ]]; then
    command -v rfu >/dev/null 2>&1 || {
        printf '%s\n' 'STATUS=BLOCKED' 'REASON=RFU_NOT_AVAILABLE' \
          'AI_INFO=NOT_GENERATED' 'CLOUD_SYNC=NOT_RUN'
        exit 20
    }
    # Log, redakce, AI_INFO, Shared Result Bus a cloud patri existujicimu wrapperu.
    # Bez wrapperu neni tichy terminal-only fallback ani novy cloud writer.
    exec rfu evidence-run --cloud-sync --wp-id PORTALMCXI-HANDOFF -- \
        bash "$(realpath -- "$0")" --evidence-child
fi

printf '%s\n' \
  'NAME=PORTALMCXI_EVIDENCE_HANDOFF' 'DATE=2026-10-07' 'VERSION=1.0.0' \
  'PURPOSE=Read-only RFU self-description a prehled potrebneho rucniho kroku' \
  "STARTED_AT=$(date -Is)" "HOST=$(hostname -s)" \
  'PORTAL_JOB_DISPATCH=NO' 'SSH_AUDIT=NO' 'DATABASE_ACCESS=NO' \
  'DEPLOY=NO' 'MERGE=NO' 'AI_MODEL_CALL=NO' 'ACCEPTED=UNCHANGED'

if [[ "$(hostname -s)" != "${PORTAL_HANDOFF_EXPECTED_HOST:-brnw11fiser}" ]]; then
    printf '%s\n' 'STATUS=BLOCKED' 'REASON=HPA_TARGET_MISMATCH'
    exit 20
fi

tmp_dir="$(mktemp -d)"
trap 'rm -rf -- "$tmp_dir"' EXIT
command -v timeout >/dev/null 2>&1 || { echo 'REASON=TIMEOUT_COMMAND_MISSING'; exit 20; }
command -v python3 >/dev/null 2>&1 || { echo 'REASON=PYTHON3_MISSING'; exit 20; }

echo 'PROGRESS=1/3 SOURCE_FRESHNESS'
if command -v git >/dev/null 2>&1; then
    # Z verejneho repozitare; nesmi vyzadovat credential prompt ani zapis do checkoutu.
    if GIT_TERMINAL_PROMPT=0 timeout 20 git ls-remote --exit-code \
        https://github.com/rosimcxi/PortalMCXI.git refs/heads/main \
        >"$tmp_dir/ref" 2>"$tmp_dir/ref-error"; then
        python3 - "$tmp_dir/ref" <<'PY'
import re, sys
line = open(sys.argv[1], encoding='utf-8').read().strip().split()
if len(line) != 2 or not re.fullmatch(r'[0-9a-f]{40}', line[0]) or line[1] != 'refs/heads/main':
    raise SystemExit('REASON=INVALID_SOURCE_REF')
print('CURRENT_PORTAL_MAIN=' + line[0])
print('SOURCE_FRESHNESS=OBSERVED_READ_ONLY')
PY
    else
        echo 'SOURCE_FRESHNESS=UNKNOWN'
    fi
else
    echo 'SOURCE_FRESHNESS=UNKNOWN_GIT_MISSING'
fi

echo 'PROGRESS=2/3 RFU_SELF_DESCRIPTION'
set +e
timeout 60 rfu -ai >"$tmp_dir/rfu-ai" 2>"$tmp_dir/rfu-error"
ai_rc=$?
set -e
printf 'RFU_AI_INFO_RC=%s\n' "$ai_rc"
# Nezverejnovat raw konfiguraci. Z JSON pouze allowlist scalar identity/status fields.
python3 - "$tmp_dir/rfu-ai" <<'PY'
import json, re, sys
try:
    data = json.load(open(sys.argv[1], encoding='utf-8'))
except (ValueError, OSError):
    print('RFU_SELF_DESCRIPTION=UNRESOLVED; REVIEW_CANONICAL_WRAPPER_LOG')
    raise SystemExit(0)
if not isinstance(data, dict):
    print('RFU_SELF_DESCRIPTION=UNEXPECTED_FORMAT')
    raise SystemExit(0)
for key in ('RFU_VERSION', 'SOURCE_SHA', 'NODE', 'PUBLISH_STATUS', 'CONTENT_SHA256'):
    value = data.get(key, data.get(key.lower()))
    if isinstance(value, (str, int, float, bool)) and re.fullmatch(r'[a-zA-Z0-9_.:-]{1,128}', str(value)):
        print(f'{key}={value}')
print('RFU_SELF_DESCRIPTION=JSON_OBSERVED')
PY

echo 'PROGRESS=3/3 HUMAN_HANDOFF_AND_CLOUD'
printf '%s\n' \
  'SOURCE_BASELINE=MERGED_TESTED_CI' \
  'TESTED_REFERENCE_SHA=74b61cd1664314aeb234459f8f69a052c3c62f23' \
  'BASELINE_PR=https://github.com/rosimcxi/PortalMCXI/pull/4' \
  'CI_REFERENCE=https://github.com/rosimcxi/PortalMCXI/actions/runs/37656849872' \
  'PROJECT_QUOTA_30_PERCENT=UNVERIFIED' \
  'CONTABO_CURRENT_RUNTIME=UNVERIFIED' \
  'NEXT_ACTION=Dolozit projektovou kvotu a autorizovany cteci kanal Contabo' \
  'DONE_CONDITION=Fresh target evidence plus quota proof; cloud wrapper footer OK' \
  'STATUS=BLOCKED_FOR_TARGET_WORK' \
  'TARGET_RC=20' "ENDED_AT=$(date -Is)"
# Technicka blokace zustava RC 20; wrapper i tak vytvori evidence a zkusi cloud-sync.
# Uspech uploadu je pouze CLOUD_SYNC_STATUS/RC z wrapperu, nikdy tato zprava.
exit 20
