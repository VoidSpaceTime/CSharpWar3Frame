## 1. State model definition

- [ ] 1.1 Define Ground / Inventory / Equipped / Stored item states
- [ ] 1.2 Define the ownership semantics of `ItemOwner`
- [ ] 1.3 Define which states apply attribute effects by default

## 2. Attribute effect flow

- [ ] 2.1 Define request-driven item effect application
- [ ] 2.2 Define request-driven item effect removal
- [ ] 2.3 Define source-based modifier mapping from item to owner attributes

## 3. TDD-oriented verification planning

- [ ] 3.1 Define tests for pickup into inventory without immediate effect
- [ ] 3.2 Define tests for equip → modifier apply
- [ ] 3.3 Define tests for unequip/drop → modifier removal
- [ ] 3.4 Define tests for stored items not affecting unit attributes

## 4. Migration planning

- [ ] 4.1 Introduce explicit item state tags/components
- [ ] 4.2 Introduce item apply/remove request flow
- [ ] 4.3 Integrate modifier source usage for equipped items
- [ ] 4.4 Remove any direct item-to-unit permanent attribute write path if introduced elsewhere

## 5. Approval gating

- [ ] 5.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 5.2 Forbid ownership-only logic from implying equipped-effect state
