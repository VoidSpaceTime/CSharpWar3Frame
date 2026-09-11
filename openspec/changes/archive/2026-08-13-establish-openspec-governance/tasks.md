## 1. Governance artifacts

- [ ] 1.1 Create and complete the OpenSpec governance change artifacts for repository-governance
- [ ] 1.2 Extend the governance spec/design/proposal to define graded proposal levels, upgrade triggers, and the unified lifecycle
- [ ] 1.3 Verify the change status shows proposal, design, specs, and tasks as recognized artifacts

## 2. Repository rule entrypoint

- [ ] 2.1 Add a root-level AGENTS.md after user approval to codify the OpenSpec-first workflow
- [ ] 2.2 Document graded proposal levels and the required approval gate before implementation
- [ ] 2.3 Document that every future code change follows design -> review -> implement -> test -> summarize -> commit

## 3. Templates and guidance

- [ ] 3.1 Add `openspec/templates/proposal-levels.md` for fast/light/full/architecture proposal scaffolding
- [ ] 3.2 Add `openspec/templates/review-checklist.md` for review gates and upgrade checks
- [ ] 3.3 Add `openspec/README.md` to document repository OpenSpec usage

## 4. Global architecture review rules

- [ ] 4.1 Define in AGENTS.md that proposals must evaluate cross-project impact across War3Frame, War3Frame.Generator, FrameBuild, CSharpWar3Frame, and Projects/*
- [ ] 4.2 Document that repository files are the source of truth and OpenViking is auxiliary memory only

## 5. Validation

- [ ] 5.1 Confirm AGENTS.md, templates, and OpenSpec README exist at the expected paths after implementation
- [ ] 5.2 Re-run repository checks to verify the governance workflow is visible and reusable
