# verify-config Audit

**Status:** DONE
Started: 2026-10-27
Completed: 2026-10-27

## Scope

Ran `.ai/skills/verify-config/SKILL.md` against the repository after the rename and the
Azure Pipelines → GitHub Actions migration.

## Results

| Check | Before | After |
|---|---|---|
| Progress files MANDATORY in `AGENTS.md` | FAIL (no mandatory wording) | PASS |
| Progress files MANDATORY in `.github/copilot-instructions.md` | FAIL (no mandatory wording) | PASS |
| `task-execution.md` tool-agnostic | FAIL (`submit_plan`, `TaskCreate`, `~/.claude/todos/`) | PASS |
| Agent roster in `AGENTS.md` / `README.md` | PASS (9/9, no Model column) | PASS |
| Agent `model:` tier aliases | PASS (`sonnet` / `haiku`) | PASS |
| Junctions `.claude/`, `.reasonix/`, `.github/` | PASS (6/6 resolve) | PASS |
| `.gitignore` junction exclusions | PASS | PASS |
| SessionStart hook contract | PASS | PASS |
| `AGENTS.md` §2 `## Session Memory` + exact strings | PASS | PASS |
| Copilot Session Memory identical | PASS | PASS |
| `.ai/session-context.md` | project handoff (kept by decision) | project handoff |
| Standards surfaces | FAIL (absent; validator exit 1) | PARTIAL — surfaces generated, validator still exit 1 on TEST 6 |

## Decisions

- **Session context**: this is a product repository, not the template, so `.ai/session-context.md`
  keeps a real handoff rather than the unfilled placeholder.
- **Standards**: generate `COMPLIANCE.md` + the marked `README.md` block with the computed
  (downgraded) row instead of claiming `Evidence` the project does not produce.
- **Root README**: replaced the AI-template overview with the project readme; the overview is kept
  as `TEMPLATE-OVERVIEW.md`, and `.github/readme.md` (now shadowed by the root file) was removed.

## Remaining failure

`python .ai/tests/validate-standards.py` → exit 1:

> ISO/IEC/IEEE 29119-3:2021 — declared `Evidence`, computed `Aligned`

The manifest's only `Evidence` row expects `docs/slices/{BC}/{Entity}.{Operation}/test-plan.md`,
`docs/test-reports/{release}-test-completion-report.md` and `docs/test-reports/incidents/TIR-*.md`.
This project produces none. Both generated surfaces record the downgrade honestly (the `Note`
column and the `*(none)*` Evidence group). Passing the check requires producing those artifacts or
re-scoping the manifest row — the manifest is template-owned.

## Files changed

- `AGENTS.md`, `.github/copilot-instructions.md` — mandatory progress-file wording
- `.ai/reference/task-execution.md` — tool-agnostic plan/tracking wording, `programmer` → `developer`
- `README.md` — project readme + generated standards block
- `TEMPLATE-OVERVIEW.md` — preserved template overview
- `.github/readme.md` — removed (duplicate, shadowed by the root README)
- `COMPLIANCE.md` — generated
- `.ai/session-context.md` — decision recorded
