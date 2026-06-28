# Refactor Playbook

1. Understand current behavior before changing structure
2. Identify the highest-value design problem
3. Make small, reversible improvements
4. Separate responsibilities before introducing abstractions
5. Replace condition-heavy logic with better modeling when justified
6. Introduce interfaces only when they reduce coupling or support variation
7. Keep public APIs stable when possible
8. Add or update tests to protect behavior
9. Prefer clarity over cleverness
10. Stop refactoring when the code becomes sufficiently understandable and maintainable

## Smells to Watch for
- Long methods/functions
- Hidden side effects
- Feature envy
- Large classes/modules with mixed responsibilities
- Inappropriate intimacy between components
- Overloaded interfaces
- Excessive branching
- Tight framework coupling
- Magic values and implicit policies
- Repeated orchestration logic spread across layers

## Refactoring heuristics
- Prefer extracting concepts over merely moving lines around
- Favor boundaries that reduce change amplification
- Keep the number of moving parts proportional to the real problem
- Improve naming before introducing patterns
- Use patterns only when they remove real friction