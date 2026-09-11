## 1. Health regen update

- [x] 1.1 Update HealthSystem to include HealthRegenPercent in the per-tick regeneration formula
- [x] 1.2 Keep health current value clamped and native dirty marking unchanged after the formula update

## 2. Mana regen update

- [x] 2.1 Update ManaSystem to include ManaRegenPercent in the per-tick regeneration formula
- [x] 2.2 Keep mana current value clamped and native dirty marking unchanged after the formula update

## 3. Verification

- [x] 3.1 Run diagnostics on the updated unit systems and confirm no new errors
- [x] 3.2 Validate that the change scope remains limited to regen formula behavior
