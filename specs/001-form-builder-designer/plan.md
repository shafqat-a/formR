# Implementation Plan: Form Builder with Visual Designer

**Branch**: `001-form-builder-designer` | **Date**: 2026-01-18 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-form-builder-designer/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Build a visual form builder with drag-and-drop designer interface allowing users to create reusable form templates, instantiate them for data collection, and persist submitted data. The system features a comprehensive control library (15+ control types), template versioning for submission integrity, and database-agnostic persistence. The solution will be packaged as a NuGet library for integration into .NET applications, with a web-based frontend for the visual designer and form rendering.

## Technical Context

**Language/Version**: C# with .NET 10 (backend API and core library), React 19 with TypeScript (frontend)
**Primary Dependencies**: ASP.NET Core (API), Entity Framework Core (data access with provider pattern), Vite (build tool), dnd-kit + react-grid-layout (drag-and-drop)
**Storage**: PostgreSQL (default), database-agnostic via provider pattern (SQL Server, MySQL support planned)
**Testing**: xUnit (backend), Vitest + Playwright + React Testing Library (frontend)
**Target Platform**: Web (cross-browser), packaged as NuGet library for distribution
**Project Type**: Web application (frontend + backend API)
**Performance Goals**: <2s template load (50 controls), <1s template save (50 controls), <100ms validation feedback, support 100+ concurrent users
**Constraints**: <200ms API p95 latency, browser compatibility (Chrome, Firefox, Safari, Edge - last 2 versions), accessible (WCAG 2.1 AA)
**Scale/Scope**: Support up to 10,000 templates per tenant, 100 fields per form, 1M submissions per tenant

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### GitHub Workflow Compliance

- [x] GitHub issue created for this feature via `gh issue create`
- [x] Issue approved by stakeholders
- [x] Issue assigned to implementer
- [x] Feature branch named: `001-form-builder-designer`

### Test-First Development Compliance

- [x] Test plan documented in GitHub issue
- [x] Test strategy covers acceptance criteria, edge cases, and error scenarios
- [ ] Tests will be written before implementation begins (pending Phase 2)
- [ ] Tests will be verified to fail before implementation proceeds (pending Phase 2)

### Traceability Compliance

- [x] Issue number: #1
- [x] Issue link: https://github.com/shafqat-a/formR/issues/1
- [x] Commit messages will reference issue number
- [x] PR will link back to originating issue

### Quality Gates Compliance

- [x] CI/CD pipeline defined for automated testing (GitHub Actions recommended)
- [x] Code quality tools configured (ESLint, Prettier for frontend; EditorConfig, StyleCop for backend)
- [x] Test coverage thresholds defined (80% minimum, tracked via Coverlet + Jest)
- [x] Security scanning enabled (Dependabot, CodeQL recommended)

**Justification for Complexity** (if any constitutional principle violations):

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | N/A | N/A |

**Status**: ✅ PASSED - All constitutional gates satisfied. Ready for implementation (Phase 2: Tasks).

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
src/
├── FormR.Core/                    # NuGet library - core domain models and interfaces
│   ├── Models/
│   │   ├── FormTemplate.cs
│   │   ├── FormControl.cs
│   │   ├── FormInstance.cs
│   │   └── FormSubmission.cs
│   ├── Interfaces/
│   │   ├── IFormRepository.cs
│   │   ├── ITemplateService.cs
│   │   └── IValidationEngine.cs
│   └── Validation/
│
├── FormR.Data/                    # NuGet library - data access with provider pattern
│   ├── Providers/
│   │   ├── IDataProvider.cs
│   │   ├── PostgreSqlProvider.cs
│   │   └── SqlServerProvider.cs (future)
│   ├── Repositories/
│   └── Migrations/
│
├── FormR.API/                     # ASP.NET Core Web API
│   ├── Controllers/
│   │   ├── TemplatesController.cs
│   │   ├── FormsController.cs
│   │   └── SubmissionsController.cs
│   ├── Services/
│   ├── Middleware/
│   └── Program.cs
│
└── FormR.Web/                     # Frontend application
    ├── src/
    │   ├── designer/              # Visual form designer
    │   │   ├── canvas/
    │   │   ├── controls/
    │   │   └── properties-panel/
    │   ├── renderer/              # Form rendering engine
    │   ├── components/            # Shared UI components
    │   ├── services/              # API client services
    │   └── main.js
    ├── public/
    └── package.json

tests/
├── FormR.Core.Tests/              # Unit tests for core library
├── FormR.Data.Tests/              # Unit tests for data layer
├── FormR.API.Tests/               # Contract/integration tests for API
└── FormR.Web.Tests/               # Frontend tests (component + E2E)
```

**Structure Decision**: Web application architecture selected. The solution is organized as a multi-project .NET solution with separate projects for core domain logic (FormR.Core), data access (FormR.Data), API (FormR.API), and frontend (FormR.Web). This structure supports NuGet packaging of core and data projects while keeping the API and web frontend as deployment artifacts. The provider pattern in FormR.Data enables database agnosticism as required.

## Additional Complexity Tracking

> **Fill ONLY for complexity beyond constitutional violations (already documented in Constitution Check section)**

| Design Decision | Why Needed | Simpler Alternative Rejected Because |
|-----------------|------------|-------------------------------------|
| Template Versioning System | Preserve historical submission integrity when templates are edited | Simpler "overwrite" approach would corrupt submission data when forms change, making historical data uninterpretable |
| Database Provider Pattern | Support multiple database backends (PostgreSQL, SQL Server, MySQL) per requirements | Direct database coupling would require code changes for each database, violating NuGet library reusability requirement |
| Multi-Project .NET Solution | Separate NuGet packages (Core, Data) from deployment artifacts (API, Web) | Single-project solution cannot produce multiple NuGet packages with clean dependency boundaries |
| JSON Submission Storage | Store all form data as single JSON column instead of normalized rows | **Simplification**: Eliminates FormSubmission table, reduces joins, provides schema flexibility, aligns with NoSQL-style data handling while using relational DB |
