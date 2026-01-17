<!--
  SYNC IMPACT REPORT

  Version Change: [INITIAL VERSION] → 1.0.0

  Modified Principles:
  - All principles newly defined (initial constitution)

  Added Sections:
  - Core Principles (5 principles)
  - Development Workflow
  - Quality & Compliance
  - Governance

  Removed Sections: None

  Templates Requiring Updates:
  ✅ .specify/templates/plan-template.md - Constitution Check section aligned
  ✅ .specify/templates/spec-template.md - Requirements and testing alignment verified
  ✅ .specify/templates/tasks-template.md - Task structure reflects workflow principles
  ✅ .specify/templates/checklist-template.md - No updates required (generic)
  ✅ .specify/templates/agent-file-template.md - No updates required (generic)

  Follow-up TODOs: None
-->

# formR Constitution

## Core Principles

### I. GitHub Development Workflow (NON-NEGOTIABLE)

All development work MUST follow GitHub's change request workflow:

- Every feature, bug fix, or change requires a GitHub issue created via `gh` CLI
- No implementation may begin without an approved change request (issue)
- Issues MUST clearly describe the problem, proposed solution, and acceptance criteria
- Pull requests MUST reference the originating issue number
- Branch naming convention: `[issue-number]-brief-description` (e.g., `123-add-user-auth`)

**Rationale**: Ensures traceability, documentation, and stakeholder alignment before code is written. Prevents wasted effort on unapproved or misunderstood requirements.

### II. Test-First Development (NON-NEGOTIABLE)

Every task MUST include a test plan before implementation:

- Test plan MUST be documented in the GitHub issue before work begins
- Tests MUST be written and verified to fail before implementation
- Implementation proceeds only after failing tests are confirmed
- Pull requests MUST demonstrate all tests passing
- Test plans MUST cover acceptance criteria, edge cases, and error scenarios

**Rationale**: Test-driven development ensures requirements are understood, edge cases are considered, and implementation correctness is provable. Failing tests first prove the tests work.

### III. Change Request Approval Gates

Work cannot proceed without proper authorization:

- Issue MUST be assigned to implementer before work begins
- Pull request MUST be reviewed and approved before merge
- Breaking changes MUST be explicitly labeled and require additional review
- No direct commits to main/master branch
- Use `gh` CLI for all GitHub operations to maintain workflow consistency

**Rationale**: Prevents unauthorized or premature work. Ensures code quality through peer review. Protects production stability.

### IV. Traceability & Documentation

Every implementation artifact MUST link back to its authorization:

- Commit messages MUST reference issue numbers (e.g., `Fixes #123: Add user authentication`)
- Pull request descriptions MUST link to the originating issue
- Test results MUST be documented in PR comments or CI output
- Changes MUST update relevant documentation (README, API docs, etc.)

**Rationale**: Maintains audit trail, enables root cause analysis, and ensures documentation stays current with code changes.

### V. Continuous Integration & Quality

Automated quality gates MUST pass before merge:

- All tests MUST pass in CI environment
- Code quality checks (linting, formatting, type checking) MUST pass
- Test coverage MUST meet project thresholds
- Build MUST succeed for all target platforms
- Security scans MUST complete without critical issues

**Rationale**: Prevents regression, ensures consistent code quality, and catches issues before they reach production.

## Development Workflow

### Standard Workflow Sequence

1. **Issue Creation**: Use `gh issue create` to document change request
2. **Issue Review**: Stakeholders review and approve/reject issue
3. **Issue Assignment**: Use `gh issue edit --add-assignee` to claim work
4. **Branch Creation**: Create feature branch following naming convention
5. **Test Plan**: Document test approach in issue comments
6. **Test Implementation**: Write failing tests first
7. **Implementation**: Write code to make tests pass
8. **Pull Request**: Use `gh pr create` linking to issue
9. **Code Review**: Reviewers approve or request changes
10. **Merge**: Merge only after approvals and CI passes
11. **Issue Closure**: Issue auto-closes on PR merge

### Pull Request Requirements

Every pull request MUST include:

- Reference to originating issue in description
- Summary of changes made
- Test plan execution results
- Documentation updates (if applicable)
- Screenshots/demos (for UI changes)
- Breaking change warnings (if applicable)

## Quality & Compliance

### Test Coverage Requirements

- Unit tests for all business logic
- Integration tests for API contracts and service interactions
- Contract tests for shared interfaces
- End-to-end tests for critical user journeys

### Code Review Standards

- All PRs require at least one approval
- Reviewers MUST verify tests exist and pass
- Reviewers MUST verify issue requirements are met
- Reviewers MUST check for security vulnerabilities
- Reviewers MUST ensure documentation is updated

### Compliance Verification

Every PR review MUST verify:

1. **Issue exists and is approved**: Verify `gh issue view [number]` shows approved status
2. **Test plan documented**: Check issue comments for test strategy
3. **Tests exist**: Verify test files are included in PR
4. **Tests pass**: Verify CI status is green
5. **Documentation updated**: Check docs/ or README for updates
6. **Constitution compliance**: Ensure no principles violated

## Governance

### Amendment Procedure

Constitution amendments require:

1. GitHub issue proposing amendment with justification
2. Discussion period (minimum 7 days for major changes)
3. Approval from project maintainers
4. Migration plan for existing workflows (if needed)
5. Version increment following semantic versioning
6. Update of all dependent templates and documentation

### Versioning Policy

Constitution versions follow semantic versioning (MAJOR.MINOR.PATCH):

- **MAJOR**: Backward-incompatible workflow changes, principle removal/redefinition
- **MINOR**: New principles added, materially expanded guidance
- **PATCH**: Clarifications, wording improvements, typo fixes

### Compliance Review

- All PRs MUST pass constitutional compliance checks
- Complexity or deviations MUST be justified in PR description
- Repeated violations trigger process review
- Constitution supersedes all other development practices

### Runtime Guidance

For day-to-day development guidance and best practices, consult:

- Issue templates in `.github/ISSUE_TEMPLATE/`
- PR templates in `.github/PULL_REQUEST_TEMPLATE.md`
- Workflow automation in `.github/workflows/`
- This constitution for non-negotiable requirements

**Version**: 1.0.0 | **Ratified**: 2026-01-18 | **Last Amended**: 2026-01-18
