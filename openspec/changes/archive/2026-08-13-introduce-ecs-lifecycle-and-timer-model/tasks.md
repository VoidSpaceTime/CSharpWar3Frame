## 1. Lifecycle state model

- [x] 1.1 Define a component-based lifecycle state model for units and other long-lived entities
- [x] 1.2 Define the boundary between persistent lifecycle state components and transient event tags

## 2. Timer task model

- [x] 2.1 Define a unified timer task component model with mode, interval, remaining, paused, owner, kind, triggerCount, and maxTriggerCount
- [x] 2.2 Define once-mode and loop-mode behavioral rules, including pause and trigger-limit semantics

## 3. Time progression architecture

- [x] 3.1 Define a timer progression system that only advances time and marks expiration
- [x] 3.2 Define that business-specific expiration side effects are handled by dedicated domain systems

## 4. Integration targets

- [x] 4.1 Map corpse retention and delayed handle cleanup onto the new lifecycle/timer model
- [x] 4.2 Map temporary skill effects, buff durations, and periodic effects onto the same timing model without using runtime callbacks
