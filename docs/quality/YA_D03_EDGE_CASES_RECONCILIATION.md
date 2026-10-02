# YA-D03 EDGE_CASES disposition checkpoint

2026-10-02 source30ed4f313bbd1cf984c30f6e31c819959339991b. Scope is seven EDGE_CASES subsections of [old snapshot](../archive/design-supplement-v0.1.1.html) section4 (lines829-859), not the entire supplement. Current v0.2/explicit decisions and [Combat core v1](../design/COMBAT_CORE_V1.md) outrank old provisional content. YA-D03 partial, not Done; no client/runtime/security changes.

| Old subsection | Current authority / disposition |
| --- | --- |
| 1 cooldown origin | REQUIREMENTS FR20 and Combat core v1 already establish official wipe event+300seconds, independent of login/catch-up call time. Clarify DETAILED_DESIGN8 with identical formula; no change to accepted rule. Account-level catch-up/admission integration remains unimplemented and must not be inferred from duel tests. |
| 2 quantization | Combat core v1 already fixes normal-attack minimum800ms then100ms ceiling. Clarify DETAILED_DESIGN5 for that implemented subset. Applying this to future casts/CD/durations/movement/consumables requires YA03 integration; do not claim tested. |
| 3 room/run boundaries | Room KO20% recovery, next-run full recovery, effect reset,5second travel, per-room resurrection/per-run item trigger are old proposals. No current duel implementation proves them. Retain as deferred integration topics; do not insert as owner-approved gameplay. |
| 4 reservations/new command | Current DETAILED_DESIGN9 already uses lease/CAS/checkpoint/outbox and section10 prohibits mid-expedition equipment/party changes. All-participant/item atomic reservation and old-command settlement still require expedition/economy integration. No DB/official-host choice accepted by this audit. |
| 5 exact8hour boundary | DETAILED_DESIGN8 says events before deadline and completed rewards at deadline; old snapshot explicitly processes completions at deadline before stopping new starts. Duel Advance has no expedition wrapper, so exact-boundary reconciliation remains open. No expiry policy silently changed here. |
| 6 RNG contract | Combat core v1 explicitly adopted old HMAC/rejection-sampling/counters for ya-duel-v1. This is already accepted implementation history, not newly deferred or redesigned. General DETAILED_DESIGN9 remains broad. No keys/security code changed; drop/gear/pity/room/run integration is not proved by duel acceptance. |
| 7 wording/data corrections | Natural page12 versus light/dark4 words and J31A2裂傷刃 need catalog source/ID/effect comparison before a content update. Old numeric economy/job coefficients remain initial proposals. No Catalog rewrite or balance approval. |

## Verification and remaining scope

Read current REQUIREMENTS/DETAILED_DESIGN/COMBAT_CORE_V1/OPEN_DECISIONS and exact old section; record actual source dispositions above. Added only two explanatory paragraphs to current design using already established FR20/Combat v1 contracts. Existing core13-test evidence is historical, not rerun. Godot/.NET not found onPATH and no Godot application found at queried location; no install or native acceptance.

Next YA-D03 unit: DATA_MODEL/ownership/economy supplement disposition against v0.2 and ONLINE_LIGHTWEIGHT D01, preserving unresolved host/cost choice. Remaining UI/operations/content/catalog/ADR integration, exact expedition boundary and catalog wording checks remain open. YA01 client test remains separate. Snapshot is readable source, not byte-exact ZIP.
