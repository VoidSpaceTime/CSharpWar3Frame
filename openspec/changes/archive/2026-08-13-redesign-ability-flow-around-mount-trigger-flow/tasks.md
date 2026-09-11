## 1. High-level model definition

- [ ] 1.1 Define Mount layer semantics
- [ ] 1.2 Define Trigger layer semantics
- [ ] 1.3 Define Flow layer semantics
- [ ] 1.4 Define Settlement layer boundaries

## 2. Mapping current systems

- [ ] 2.1 Map current slot abilities into the new Mount layer
- [ ] 2.2 Map non-slot-mounted talents/passives into the new Mount layer
- [ ] 2.3 Map current effect payload components into flow node semantics
- [ ] 2.4 Map current damage/heal/buff/contribution systems into settlement layers

## 3. Verification planning

- [ ] 3.1 Verify an active cast ability fits the model
- [ ] 3.2 Verify a damage-triggered passive ability fits the model
- [ ] 3.3 Verify a death-triggered ability fits the model
- [ ] 3.4 Verify a composite ability such as lava ball + ground field fits the model

## 4. Approval gating

- [ ] 4.1 Block implementation until proposal, design, spec, and tasks review is approved
- [ ] 4.2 Forbid treating payload components as the whole ability model
- [ ] 4.3 Forbid mixing trigger semantics and settlement semantics into one undifferentiated layer
