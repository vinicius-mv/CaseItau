# Review Checklist

## Design

- Are responsibilities well separated?
- Does each class/module/function have a clear purpose?
- Are abstractions meaningful and necessary?
- Is dependency direction correct?

## SOLID

- Is there any SRP violation?
- Is there a better extension mechanism instead of modifying stable code?
- Can implementations be substituted safely?
- Are interfaces overly broad?
- Does business logic depend on infrastructure details?

## Maintainability

- Are names intention-revealing?
- Is the code readable without excessive mental load?
- Is there avoidable duplication?
- Are edge cases handled clearly?

## Testing

- Is the design easy to test?
- Are tests validating behavior rather than implementation details?
- Are boundaries exercised by integration tests where needed?

## Architecture

- Are domain rules isolated from frameworks and infrastructure?
- Is there any leakage of transport/persistence concerns into business logic?
- Are module boundaries explicit and coherent?

## Delivery Quality

- Are error paths and failure modes explicit?
- Is logging/observability placed at correct boundaries?
- Is there a safe migration path if changes affect existing behavior?
