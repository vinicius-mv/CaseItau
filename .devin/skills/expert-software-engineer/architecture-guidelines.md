# Architecture Guidelines

## General
- Keep domain rules independent from infrastructure and framework details
- Prefer explicit modules boundaries
- Make dependencies visible and intentional
- Avoid leaking transport/database concerns into domain logic

## Application Layer
- Coordinate use cases
- Orchestrate domain operations
- Avoid placing business rules in controllers, handlers, or adapters
- Keep workflows explicit and easy to test

## Domain Layer
- Contain business rules and core invariants
- Avoid framework annotations or infrastructure dependencies when possible
- Models concepts using intention-revealing names
- Protect invariants close to the domain

## Infrastructure Layer
- Implement ports/interfaces defined by the core
- Keep external system concerns isolated
- Avoid letting persistence models dictate domain models
- Encapsulate integration details and retries near the edge

## API Design
- Use clear and stable contracts
- Prefer explicit input/output models
- Design error handling consistently
- Optimize for consumer understanding, not only backend convenience

## Dependency Injection
- Outer layers may depend on inner layers
- Inner layers must not depend on frameworks, UI, transport, or persistence details
- Use abstractions to invert dependencies when infrastructure must integrate with core use cases

## Testing Strategy by Layer
- Domain: mostly unit tests
- Application: unit + targeted integration tests
- Infrastructure: integration tests
- System Behavior: a small number of high-value end-to-end tests