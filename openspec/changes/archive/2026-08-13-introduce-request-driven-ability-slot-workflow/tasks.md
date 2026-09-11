## 1. Request model definition

- [ ] 1.1 Define `AbilityAttachRequest`
- [ ] 1.2 Define `AbilityRemoveRequest`
- [ ] 1.3 Define `AbilitySwapRequest`
- [ ] 1.4 Define `AbilitySlotResizeRequest`

## 2. Workflow ownership definition

- [ ] 2.1 Define the canonical immediate workflow system for slot mutations
- [ ] 2.2 Define which validations belong to the workflow system
- [ ] 2.3 Define which contribution requests are triggered at attach/remove time

## 3. Verification planning

- [ ] 3.1 Verify attach flow
- [ ] 3.2 Verify remove flow
- [ ] 3.3 Verify swap flow
- [ ] 3.4 Verify resize flow
- [ ] 3.5 Verify contribution apply/remove timing remains correct

## 4. Approval gating

- [ ] 4.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 4.2 Forbid helper-owned full slot workflow semantics
- [ ] 4.3 Forbid periodic polling as the main slot workflow mechanism
