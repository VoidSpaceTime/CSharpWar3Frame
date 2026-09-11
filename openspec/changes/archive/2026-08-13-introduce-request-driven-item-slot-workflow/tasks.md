## 1. Request model definition

- [ ] 1.1 Define `ItemAttachRequest`
- [ ] 1.2 Define `ItemRemoveRequest`
- [ ] 1.3 Define `ItemSwapRequest`
- [ ] 1.4 Define `ItemSlotResizeRequest`

## 2. Workflow ownership definition

- [ ] 2.1 Define `ItemSlotContainer`
- [ ] 2.2 Define canonical immediate workflow ownership for item slot mutations
- [ ] 2.3 Define interaction between slot changes and item attr apply/remove requests

## 3. Verification planning

- [ ] 3.1 Verify attach flow
- [ ] 3.2 Verify remove flow
- [ ] 3.3 Verify swap flow
- [ ] 3.4 Verify resize flow
- [ ] 3.5 Verify slot changes do not accidentally imply equipped effects

## 4. Approval gating

- [ ] 4.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 4.2 Forbid slot presence from implicitly meaning equipped effect activation
