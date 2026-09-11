## 1. Architecture definition

- [ ] 1.1 Define canonical ECS data for move command, execution state, outcome, and continuation
- [ ] 1.2 Define command publishing, native execution, and execution monitoring ownership boundaries
- [ ] 1.3 Define outcome taxonomy for arrived / cancelled / overridden / interrupted / failed
- [ ] 1.4 Define caller-owned continuation semantics for casting, preset tasks, interaction, and AI

## 2. TDD-oriented verification planning

- [ ] 2.1 Define red-first tests for basic arrival through arrivalDistance thresholds
- [ ] 2.2 Define red-first tests for move-then-cast workflows
- [ ] 2.3 Define red-first tests for move-then-task workflows
- [ ] 2.4 Define red-first tests for player override via new commands
- [ ] 2.5 Define red-first tests for Stop / Hold override semantics
- [ ] 2.6 Define red-first tests for control-effect interruption semantics
- [ ] 2.7 Define red-first tests for target movement updates and reissue rules

## 3. Migration sequencing

- [ ] 3.1 Introduce canonical move data model and monitoring semantics
- [ ] 3.2 Migrate casting pre-move to caller-based use of the move subsystem
- [ ] 3.3 Migrate preset task and AI movement to the same move subsystem
- [ ] 3.4 Add override/interruption observation and outcome emission
- [ ] 3.5 Remove old `MoveToCast`-centric ownership after verification passes

## 4. Cross-project impact review

- [ ] 4.1 Document direct runtime impact within `War3Frame`
- [ ] 4.2 Review `War3Frame.Generator` for generated registration or API assumptions
- [ ] 4.3 Review `FrameBuild` for runtime wiring assumptions
- [ ] 4.4 Review `CSharpWar3Frame` for CLI/tooling assumptions
- [ ] 4.5 Review `Projects/*` for scenario scripts, tests, and authored workflows relying on old move semantics

## 5. Approval and execution gating

- [ ] 5.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 5.2 Require TDD-first execution
- [ ] 5.3 Forbid casting-owned move semantics
- [ ] 5.4 Forbid move subsystem from hard-coding caller continuations
- [ ] 5.5 Forbid native execution from becoming the semantic truth owner

## 6. Atomic commit strategy

- [ ] 6.1 Plan one OpenSpec-only proposal commit
- [ ] 6.2 Plan one red-tests-only commit
- [ ] 6.3 Plan one canonical move data model commit
- [ ] 6.4 Plan one native execution and monitoring commit
- [ ] 6.5 Plan one casting integration commit
- [ ] 6.6 Plan one task/AI integration commit
- [ ] 6.7 Plan one legacy ownership cleanup commit after verification passes
