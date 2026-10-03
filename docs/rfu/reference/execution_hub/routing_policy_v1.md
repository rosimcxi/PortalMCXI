# RFU Execution Hub V1 routing

Authority split:
- RFU Automation owns what/when/dependencies/gates/result destination.
- RFU AI owns provider/model/executor routing and normalized AI result handling.
- WSL bridge is a node adapter, not a second scheduler.
- RFU Identity / node registry #272 owns live availability; Execution Hub #207 consumes it.

## HPA normal queue / fast reserved recheck — APPROVED 2026-09-25

Temporary policy #284 expired automatically at 2026-09-24T23:59:59+02:00.
HPA is now NORMAL, not RESERVED.

Before every new HPA handoff:
1. verify fresh Identity availability, capability, trust and capacity;
2. check reserved work: URGENT, then SEN;
3. otherwise continue standard Execution Hub placement.

Reserved classification:
- URGENT = explicit P0/P1 urgent policy/tag/marker;
- SEN = explicit canonical SEN domain/capability/work-package identity;
- never infer either from arbitrary free text.

Normal behavior:
- if reserved work is empty, HPA may take one normal eligible job;
- after one normal/fallback job, RECHECK_REQUIRED before another normal claim;
- generic Linux/build/CI remains preferably routed to CONTABO when equally capable;
- HPA remains preferred/required for corporate/VPN/SEN/INX/PG/db.readonly-target work when capability requires it.

Fast preemption:
- recheck reserved work about every 60 seconds while a normal PREEMPT_SAFE job runs;
- if reserved work appears and estimated remaining time is >5 minutes, checkpoint/hibernate/requeue and switch;
- if remaining time is <=5 minutes, finish then immediately recheck;
- DB write/migration/transaction and other non-preempt-safe work are not automatically interrupted;
- after reserved work, checkpointed work may enter RESUMING via normal placement.

No parallel scheduler:
- Execution Hub remains the only placement/routing authority;
- WSL bridge and CI runners are executor adapters only;
- no separate HPA queue daemon or second node registry.

Default routing remains cheapest-capable-first with capability/trust/capacity checks. Offline or saturated preferred nodes release work to another eligible executor or WAITING; no blind rerun loop.

