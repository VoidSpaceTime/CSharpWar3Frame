## 1. Creation request model

- [x] 1.1 Define the ECS-side component or tag set that represents a pending native unit creation request
- [x] 1.2 Define the completion-state rule that prevents duplicate native unit creation after the request is consumed

## 2. Entry path refactor

- [x] 2.1 Update the unit creation entry path so UnitHelper no longer directly triggers native unit creation
- [x] 2.2 Adjust template-based creation to produce an entity plus creation request data instead of directly calling War3 native creation

## 3. Immediate native creation

- [x] 3.1 Add an ImmediateRoot system that consumes pending native unit creation requests and creates the War3 native unit immediately
- [x] 3.2 Attach UnitNative and required native-side initialization data back onto the entity after creation

## 4. Lifecycle verification

- [x] 4.1 Audit call sites that assume UnitNative exists immediately after creation and align them to the new timing model
- [x] 4.2 Verify unit creation, native data access, and native death handling all work under the unified request-driven lifecycle
