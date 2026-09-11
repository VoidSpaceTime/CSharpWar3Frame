## 1. Governance spec clarification

- [x] 1.1 Clarify in `repository-governance/spec.md` that every proposal level records its approved scope in `openspec/changes/<change-id>/proposal.md`
- [x] 1.2 Add a single artifact matrix defining required and optional artifacts for `fast`, `light`, `full`, and `architecture`
- [x] 1.3 Define conflict resolution priority when spec, AGENTS, README, and templates use different wording

## 2. Governance entrypoint alignment

- [x] 2.1 Update `AGENTS.md` so fast/light levels explicitly use `proposal.md` as the standard record location
- [x] 2.2 Update `AGENTS.md` wording so low-level changes do not imply mandatory four-piece artifact sets

## 3. Guidance and template alignment

- [x] 3.1 Update `openspec/README.md` to describe the level-to-artifact matrix without implying all levels require full artifact sets
- [x] 3.2 Update `openspec/templates/proposal-levels.md` to explain when light/full optional and required artifacts apply
- [x] 3.3 Update `openspec/templates/review-checklist.md` to verify proposal record location and prevent unintended artifact inflation or under-documentation

## 4. Validation

- [x] 4.1 Verify all clarified documents use one consistent artifact matrix
- [x] 4.2 Verify the new change remains proposal-only and does not introduce implementation beyond governance text
