# Repository Onboarding Checklist

## Documentation

- [ ] Read `README.md`
- [ ] Read `CONTRIBUTING.md` if present
- [ ] Inspect docs folders and onboarding docs
- [ ] Inspect any architecture or ADR documents

## Tooling and Runtime

- [ ] Identify package manager(s)
- [ ] Identify language/runtime versions
- [ ] Identify build tools
- [ ] Identify test frameworks
- [ ] Identify lint/format/typecheck tooling
- [ ] Identify environment setup requirements

## Scripts and Commands

- [ ] Inspect root scripts and task runners
- [ ] Inspect package scripts / build definitions / make targets
- [ ] Identify bootstrap/install sequence
- [ ] Identify build sequence
- [ ] Identify test sequence
- [ ] Identify lint/format/typecheck sequence
- [ ] Identify local run/debug commands

## Validation

- [ ] Run the minimal install/bootstrap path
- [ ] Run build
- [ ] Run lint
- [ ] Run tests
- [ ] Run typecheck if applicable
- [ ] Note exact command ordering that works
- [ ] Record failures, warnings, and workarounds
- [ ] Record timeout-sensitive commands and approximate runtime

## CI / Automation

- [ ] Inspect `.github/workflows/`
- [ ] Record which checks run in CI
- [ ] Map CI steps to local commands where possible
- [ ] Note any missing local equivalents

## Architecture and Layout

- [ ] Summarize repository purpose
- [ ] Summarize stack and runtimes
- [ ] Map key modules and boundaries
- [ ] Identify entrypoints
- [ ] Identify config files
- [ ] Identify test locations
- [ ] Identify where common changes should be made

## AGENTS.md Output Quality

- [ ] Keep the final guidance concise
- [ ] Avoid task-specific instructions
- [ ] Include only stable, high-value operational facts
- [ ] Explicitly document reliable command order
