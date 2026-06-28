---
name: repo-onboarding
description: Inventories the repository, validates build/test/lint/run workflows, and produces or updates concise Windsurf-native repository guidance in AGENTS.md.
---

# Repository Onboarding Skill

Use this skill when you need to onboard a repository for Windsurf/Cascade so future work can be completed with minimal exploration and fewer command failures.

## Primary Objective

Create or update repository guidance that helps Cascade work efficiently the first time it sees the codebase.

The output of this skill should normally be:

- an update to the root `AGENTS.md`, or
- a proposed `AGENTS.md` section for the user to review and paste

## High-Level Goals

- Reduce the likelihood of producing changes that fail CI, validation pipelines, linting, tests, or local build steps
- Minimize shell command failures
- Reduce the need for repeated exploration with search tools
- Capture the highest-value repository facts in a concise, reusable format

## Instructions

Perform a comprehensive repository inventory before writing or updating guidance.

### 1. Repository Inventory

Inspect and summarize:

- `README.md`
- `CONTRIBUTING.md`
- docs directories and onboarding docs
- build scripts
- package manager files
- task runners
- project manifests
- compiler, formatter, linter, and test configuration files
- GitHub Actions and other CI/CD workflows
- docker/devcontainer/bootstrap environment files
- Makefiles, shell scripts, task definitions, and repo setup helpers

Search for:

- build steps
- validation steps
- command aliases
- required runtimes and versions
- workarounds
- known sharp edges
- comments such as `TODO`, `FIXME`, `HACK`, `WORKAROUND`, `NOTE`

### 2. Command Validation

When possible, validate the real command sequence for:

- bootstrap
- install
- build
- test
- lint
- run
- typecheck
- format
- packaging or release-related verification

Document:

- which commands work
- the exact order they should be run
- required tools and versions
- preconditions
- postconditions
- environment variables that are actually required
- command failures and error messages
- workarounds
- timing or timeout issues
- whether a clean repo or clean environment changes behavior

Always prefer documenting the minimal reliable sequence that future runs should follow.

### 3. Architecture and Layout Mapping

Document the project in a way that reduces future searching:

- what the repository does
- primary languages/frameworks/runtimes
- repo type and major modules
- important root files
- main source roots
- test locations
- configuration file locations
- CI workflow locations
- key entrypoints
- where to make common types of changes

### 4. Validation and Check-in Guidance

Document:

- checks that should be run before proposing changes
- which local commands best approximate CI
- any order dependencies among checks
- any validation steps mentioned in docs or workflows
- dependencies that are not obvious from file layout

### 5. Output Constraints

The final persistent guidance written into `AGENTS.md` must be concise and repository-agnostic enough to stay useful over time:

- keep it short
- do not include task-specific instructions
- prefer stable facts over transient observations
- include only details that reduce repeated searching or command failure

## Writing Requirements

When producing onboarding guidance for `AGENTS.md`, organize it into these sections when applicable:

1. Repository Summary
2. Stack and Runtime
3. Reliable Commands
4. Validation Order
5. Architecture Map
6. Key Files and Directories
7. CI / Pipeline Notes
8. Known Gotchas
9. Guidance for Future Agents

## Final Rule

Trust the repository guidance already present in `AGENTS.md` unless it is clearly incomplete, outdated, or contradicted by the codebase or validation results.

Only perform additional searching when:

- the instructions are incomplete
- the instructions are incorrect
- the repository structure has changed
- a command behaves differently from the documented behavior
