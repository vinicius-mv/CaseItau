# AGENTS.md Onboarding Section Template

## Reliable Commands

### Run Locally

# commands here

...

## Validation Order

Run checks in this order unless the repository documents a stricter sequence:

1. bootstrap/install
2. lint / format / typecheck
3. build
4. tests
5. any repo-specific validation

## Architecture Map

- Main entrypoint(s):
- Main application module(s):
- Domain/business logic location:
- Infrastructure/integration layer location:
- API/web layer location:
- Test directories:
- Config locations:

## Key Files and Directories

- `/...`
- `/...`
- `/...`

## CI / Pipeline Notes

- Main workflows:
- Checks mirrored locally:
- Checks with no good local equivalent:
- Required pre-PR validation:

## Known Gotchas

- Always ...
- Never ...
- If X fails, do Y ...
- This command requires ...
- This environment variable is actually required ...

## Guidance for Future Agents

Trust the guidance in this file first.

Only search further if:

- the instructions are incomplete
- the instructions are contradicted by the repository
- a validated command fails unexpectedly
