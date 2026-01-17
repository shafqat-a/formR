# Tasks: Form Builder with Visual Designer

**Input**: Design documents from `/specs/001-form-builder-designer/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**GitHub Issue**: #1 - https://github.com/shafqat-a/formR/issues/1
**Feature Branch**: `001-form-builder-designer`

**Tests**: Tests are REQUIRED per constitution - they MUST be written first and verified to fail before implementation.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

Based on plan.md structure:
- **Backend**: `src/FormR.Core/`, `src/FormR.Data/`, `src/FormR.API/`
- **Frontend**: `src/FormR.Web/`
- **Tests**: `tests/FormR.Core.Tests/`, `tests/FormR.Data.Tests/`, `tests/FormR.API.Tests/`, `tests/FormR.Web.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Create .NET solution structure with FormR.Core, FormR.Data, FormR.API projects per plan.md
- [ ] T002 [P] Install NuGet packages for FormR.Core (FluentValidation, System.Text.Json)
- [ ] T003 [P] Install NuGet packages for FormR.Data (EF Core, Npgsql.EntityFrameworkCore.PostgreSQL)
- [ ] T004 [P] Install NuGet packages for FormR.API (Swashbuckle, JwtBearer, AspNetCoreRateLimit)
- [ ] T005 [P] Create xUnit test projects (FormR.Core.Tests, FormR.Data.Tests, FormR.API.Tests)
- [ ] T006 [P] Install test packages (FluentAssertions, Testcontainers.PostgreSql, WebApplicationFactory)
- [ ] T007 Initialize React frontend with Vite at src/FormR.Web/
- [ ] T008 [P] Install frontend dependencies (React 19, TypeScript, dnd-kit, react-grid-layout, axios)
- [ ] T009 [P] Install frontend testing packages (Vitest, Testing Library, Playwright)
- [ ] T010 [P] Configure ESLint and Prettier for frontend in src/FormR.Web/
- [ ] T011 [P] Configure EditorConfig for backend projects
- [ ] T012 [P] Setup PostgreSQL via Docker or local installation
- [ ] T013 [P] Create appsettings.json with connection strings in src/FormR.API/
- [ ] T014 [P] Configure Vite proxy for API in src/FormR.Web/vite.config.ts
- [ ] T015 [P] Setup GitHub Actions CI/CD workflow in .github/workflows/ci.yml
- [ ] T016 [P] Configure Dependabot in .github/dependabot.yml
- [ ] T017 [P] Setup code coverage reporting (Coverlet for backend, c8 for frontend)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Core Domain Models & Enums

- [ ] T018 [P] Create ControlType enum in src/FormR.Core/Models/ControlType.cs
- [ ] T019 [P] Create InstanceStatus enum in src/FormR.Core/Models/InstanceStatus.cs
- [ ] T020 [P] Create ControlCategory enum in src/FormR.Core/Models/ControlCategory.cs

### Database Context & Provider Pattern

- [ ] T021 Create IDataProvider interface in src/FormR.Data/Providers/IDataProvider.cs
- [ ] T022 Create FormBuilderContext DbContext in src/FormR.Data/FormBuilderContext.cs
- [ ] T023 Create PostgreSqlProvider in src/FormR.Data/Providers/PostgreSqlProvider.cs
- [ ] T024 Configure provider registration in src/FormR.API/Program.cs

### Core Interfaces

- [ ] T025 [P] Create IFormRepository interface in src/FormR.Core/Interfaces/IFormRepository.cs
- [ ] T026 [P] Create ITemplateService interface in src/FormR.Core/Interfaces/ITemplateService.cs
- [ ] T027 [P] Create IValidationEngine interface in src/FormR.Core/Interfaces/IValidationEngine.cs

### API Infrastructure

- [ ] T028 [P] Configure JWT authentication middleware in src/FormR.API/Program.cs
- [ ] T029 [P] Configure multi-tenancy middleware in src/FormR.API/Middleware/TenantResolutionMiddleware.cs
- [ ] T030 [P] Configure CORS in src/FormR.API/Program.cs
- [ ] T031 [P] Configure Swagger/OpenAPI in src/FormR.API/Program.cs
- [ ] T032 [P] Configure rate limiting in src/FormR.API/Program.cs
- [ ] T033 [P] Configure global error handling middleware in src/FormR.API/Middleware/ErrorHandlingMiddleware.cs

### Frontend Infrastructure

- [ ] T034 [P] Create API client service base in src/FormR.Web/src/services/api-client.ts
- [ ] T035 [P] Setup Axios interceptors for auth in src/FormR.Web/src/services/interceptors.ts
- [ ] T036 [P] Create error handling utilities in src/FormR.Web/src/utils/error-handler.ts

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Create Form Template with Visual Designer (Priority: P1) 🎯 MVP

**Goal**: Enable users to create form templates using drag-and-drop visual designer with control library

**Independent Test**: Create new template, add controls (text, dropdown, checkbox), arrange them, configure properties, save template, verify template can be retrieved with exact configuration

### Tests for User Story 1 (Write First, Verify Failure)

- [ ] T037 [P] [US1] Unit test for FormTemplate validation in tests/FormR.Core.Tests/Models/FormTemplateTests.cs
- [ ] T038 [P] [US1] Unit test for FormControl validation in tests/FormR.Core.Tests/Models/FormControlTests.cs
- [ ] T039 [P] [US1] Unit test for ControlLibrary seeding in tests/FormR.Data.Tests/SeedDataTests.cs
- [ ] T040 [P] [US1] Contract test for POST /v1/templates in tests/FormR.API.Tests/Controllers/TemplatesControllerTests.cs
- [ ] T041 [P] [US1] Contract test for GET /v1/templates in tests/FormR.API.Tests/Controllers/TemplatesControllerTests.cs
- [ ] T042 [P] [US1] Contract test for GET /v1/templates/{id} in tests/FormR.API.Tests/Controllers/TemplatesControllerTests.cs
- [ ] T043 [P] [US1] Contract test for GET /v1/controls in tests/FormR.API.Tests/Controllers/ControlsControllerTests.cs
- [ ] T044 [P] [US1] Component test for Canvas component in tests/FormR.Web.Tests/designer/Canvas.test.tsx
- [ ] T045 [P] [US1] Component test for ControlPalette in tests/FormR.Web.Tests/designer/ControlPalette.test.tsx
- [ ] T046 [P] [US1] Component test for PropertiesPanel in tests/FormR.Web.Tests/designer/PropertiesPanel.test.tsx
- [ ] T047 [US1] E2E test for template creation flow in tests/FormR.Web.Tests/e2e/create-template.spec.ts

### Backend Models for User Story 1

- [ ] T048 [P] [US1] Create FormTemplate model in src/FormR.Core/Models/FormTemplate.cs
- [ ] T049 [P] [US1] Create FormControl model in src/FormR.Core/Models/FormControl.cs
- [ ] T050 [P] [US1] Create ControlLibrary model in src/FormR.Core/Models/ControlLibrary.cs

### Backend Validation for User Story 1

- [ ] T051 [P] [US1] Create TemplateValidator using FluentValidation in src/FormR.Core/Validation/TemplateValidator.cs
- [ ] T052 [P] [US1] Create ControlValidator using FluentValidation in src/FormR.Core/Validation/ControlValidator.cs

### Database Configuration for User Story 1

- [ ] T053 [US1] Configure FormTemplate entity mapping in src/FormR.Data/FormBuilderContext.cs
- [ ] T054 [US1] Configure FormControl entity mapping in src/FormR.Data/FormBuilderContext.cs
- [ ] T055 [US1] Configure ControlLibrary entity mapping in src/FormR.Data/FormBuilderContext.cs
- [ ] T056 [US1] Create ControlLibrary seed data migration in src/FormR.Data/Migrations/
- [ ] T057 [US1] Create initial database migration for templates and controls in src/FormR.Data/Migrations/

### Backend Services & Repositories for User Story 1

- [ ] T058 [US1] Implement FormRepository in src/FormR.Data/Repositories/FormRepository.cs
- [ ] T059 [US1] Implement TemplateService in src/FormR.API/Services/TemplateService.cs

### Backend API Endpoints for User Story 1

- [ ] T060 [US1] Implement TemplatesController.List (GET /v1/templates) in src/FormR.API/Controllers/TemplatesController.cs
- [ ] T061 [US1] Implement TemplatesController.Create (POST /v1/templates) in src/FormR.API/Controllers/TemplatesController.cs
- [ ] T062 [US1] Implement TemplatesController.Get (GET /v1/templates/{id}) in src/FormR.API/Controllers/TemplatesController.cs
- [ ] T063 [P] [US1] Implement ControlsController.GetLibrary (GET /v1/controls) in src/FormR.API/Controllers/ControlsController.cs

### Frontend Services for User Story 1

- [ ] T064 [P] [US1] Create TemplateService API client in src/FormR.Web/src/services/template-service.ts
- [ ] T065 [P] [US1] Create ControlLibraryService API client in src/FormR.Web/src/services/control-library-service.ts

### Frontend State Management for User Story 1

- [ ] T066 [US1] Create template designer state store in src/FormR.Web/src/stores/designer-store.ts

### Frontend Components - Control Palette for User Story 1

- [ ] T067 [P] [US1] Create ControlPalette component in src/FormR.Web/src/designer/controls/ControlPalette.tsx
- [ ] T068 [P] [US1] Create DraggableControl component in src/FormR.Web/src/designer/controls/DraggableControl.tsx
- [ ] T069 [P] [US1] Create ControlCategory component in src/FormR.Web/src/designer/controls/ControlCategory.tsx

### Frontend Components - Canvas for User Story 1

- [ ] T070 [US1] Create DesignerCanvas component with dnd-kit in src/FormR.Web/src/designer/canvas/DesignerCanvas.tsx
- [ ] T071 [US1] Implement DroppableCanvas area in src/FormR.Web/src/designer/canvas/DroppableCanvas.tsx
- [ ] T072 [US1] Create CanvasControl component in src/FormR.Web/src/designer/canvas/CanvasControl.tsx
- [ ] T073 [US1] Implement grid snapping with react-grid-layout in src/FormR.Web/src/designer/canvas/GridLayout.tsx
- [ ] T074 [P] [US1] Create visual guides component in src/FormR.Web/src/designer/canvas/VisualGuides.tsx

### Frontend Components - Properties Panel for User Story 1

- [ ] T075 [US1] Create PropertiesPanel component in src/FormR.Web/src/designer/properties-panel/PropertiesPanel.tsx
- [ ] T076 [P] [US1] Create LabelEditor in src/FormR.Web/src/designer/properties-panel/LabelEditor.tsx
- [ ] T077 [P] [US1] Create ValidationRulesEditor in src/FormR.Web/src/designer/properties-panel/ValidationRulesEditor.tsx
- [ ] T078 [P] [US1] Create PositionEditor in src/FormR.Web/src/designer/properties-panel/PositionEditor.tsx
- [ ] T079 [P] [US1] Create control-specific property editors in src/FormR.Web/src/designer/properties-panel/control-editors/

### Frontend Main Designer Page for User Story 1

- [ ] T080 [US1] Create DesignerPage component in src/FormR.Web/src/pages/DesignerPage.tsx
- [ ] T081 [US1] Create TemplateListPage component in src/FormR.Web/src/pages/TemplateListPage.tsx
- [ ] T082 [US1] Implement template save functionality with API integration
- [ ] T083 [US1] Implement template load functionality with API integration

### Integration for User Story 1

- [ ] T084 [US1] Wire up designer page routing in src/FormR.Web/src/App.tsx
- [ ] T085 [US1] Test complete designer flow (create, configure, save, reload template)
- [ ] T086 [US1] Verify all US1 tests pass

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently. Users can create and save form templates visually.

---

## Phase 4: User Story 2 - Instantiate Template and Enter Data (Priority: P2)

**Goal**: Enable users to select a template, create form instance, fill out data, submit, and view submissions

**Independent Test**: Select existing template, create instance, fill all fields with valid data, submit form, retrieve submission data and verify all fields match

### Tests for User Story 2 (Write First, Verify Failure)

- [ ] T087 [P] [US2] Unit test for FormInstance validation in tests/FormR.Core.Tests/Models/FormInstanceTests.cs
- [ ] T088 [P] [US2] Unit test for submission data JSON validation in tests/FormR.Core.Tests/Validation/SubmissionValidatorTests.cs
- [ ] T089 [P] [US2] Contract test for POST /v1/forms in tests/FormR.API.Tests/Controllers/FormsControllerTests.cs
- [ ] T090 [P] [US2] Contract test for POST /v1/forms/{id}/submit in tests/FormR.API.Tests/Controllers/FormsControllerTests.cs
- [ ] T091 [P] [US2] Contract test for GET /v1/submissions in tests/FormR.API.Tests/Controllers/SubmissionsControllerTests.cs
- [ ] T092 [P] [US2] Contract test for GET /v1/submissions/{id} in tests/FormR.API.Tests/Controllers/SubmissionsControllerTests.cs
- [ ] T093 [P] [US2] Component test for FormRenderer in tests/FormR.Web.Tests/renderer/FormRenderer.test.tsx
- [ ] T094 [P] [US2] Component test for SubmissionsList in tests/FormR.Web.Tests/submissions/SubmissionsList.test.tsx
- [ ] T095 [US2] E2E test for form fill and submit flow in tests/FormR.Web.Tests/e2e/submit-form.spec.ts

### Backend Models for User Story 2

- [ ] T096 [US2] Create FormInstance model with SubmissionData JSON property in src/FormR.Core/Models/FormInstance.cs

### Backend Validation for User Story 2

- [ ] T097 [US2] Create SubmissionValidator in src/FormR.Core/Validation/SubmissionValidator.cs
- [ ] T098 [US2] Implement ValidationEngine in src/FormR.Core/Validation/ValidationEngine.cs

### Database Configuration for User Story 2

- [ ] T099 [US2] Configure FormInstance entity mapping with JSONB column in src/FormR.Data/FormBuilderContext.cs
- [ ] T100 [US2] Create migration for FormInstance table in src/FormR.Data/Migrations/
- [ ] T101 [US2] Add JSONB indexes for PostgreSQL queries in migration

### Backend Services for User Story 2

- [ ] T102 [US2] Implement FormInstanceService in src/FormR.API/Services/FormInstanceService.cs
- [ ] T103 [US2] Implement SubmissionService in src/FormR.API/Services/SubmissionService.cs

### Backend API Endpoints for User Story 2

- [ ] T104 [US2] Implement FormsController.Create (POST /v1/forms) in src/FormR.API/Controllers/FormsController.cs
- [ ] T105 [US2] Implement FormsController.Get (GET /v1/forms/{id}) in src/FormR.API/Controllers/FormsController.cs
- [ ] T106 [US2] Implement FormsController.Submit (POST /v1/forms/{id}/submit) in src/FormR.API/Controllers/FormsController.cs
- [ ] T107 [US2] Implement SubmissionsController.List (GET /v1/submissions) in src/FormR.API/Controllers/SubmissionsController.cs
- [ ] T108 [US2] Implement SubmissionsController.Get (GET /v1/submissions/{id}) in src/FormR.API/Controllers/SubmissionsController.cs

### Frontend Services for User Story 2

- [ ] T109 [P] [US2] Create FormInstanceService API client in src/FormR.Web/src/services/form-instance-service.ts
- [ ] T110 [P] [US2] Create SubmissionService API client in src/FormR.Web/src/services/submission-service.ts

### Frontend Components - Form Renderer for User Story 2

- [ ] T111 [US2] Create FormRenderer component in src/FormR.Web/src/renderer/FormRenderer.tsx
- [ ] T112 [US2] Create DynamicControl component factory in src/FormR.Web/src/renderer/DynamicControl.tsx
- [ ] T113 [P] [US2] Create TextInput control in src/FormR.Web/src/renderer/controls/TextInput.tsx
- [ ] T114 [P] [US2] Create NumberInput control in src/FormR.Web/src/renderer/controls/NumberInput.tsx
- [ ] T115 [P] [US2] Create Dropdown control in src/FormR.Web/src/renderer/controls/Dropdown.tsx
- [ ] T116 [P] [US2] Create Checkbox control in src/FormR.Web/src/renderer/controls/Checkbox.tsx
- [ ] T117 [P] [US2] Create RadioGroup control in src/FormR.Web/src/renderer/controls/RadioGroup.tsx
- [ ] T118 [P] [US2] Create DatePicker control in src/FormR.Web/src/renderer/controls/DatePicker.tsx
- [ ] T119 [US2] Implement real-time validation in renderer
- [ ] T120 [US2] Implement validation error display

### Frontend Components - Submissions for User Story 2

- [ ] T121 [P] [US2] Create SubmissionsList component in src/FormR.Web/src/submissions/SubmissionsList.tsx
- [ ] T122 [P] [US2] Create SubmissionDetail component in src/FormR.Web/src/submissions/SubmissionDetail.tsx
- [ ] T123 [P] [US2] Create pagination component in src/FormR.Web/src/components/Pagination.tsx

### Frontend Pages for User Story 2

- [ ] T124 [US2] Create FormFillPage component in src/FormR.Web/src/pages/FormFillPage.tsx
- [ ] T125 [US2] Create SubmissionsPage component in src/FormR.Web/src/pages/SubmissionsPage.tsx
- [ ] T126 [US2] Implement form submission with API integration
- [ ] T127 [US2] Implement submission retrieval with API integration

### Integration for User Story 2

- [ ] T128 [US2] Wire up form fill and submissions routing in src/FormR.Web/src/App.tsx
- [ ] T129 [US2] Test complete form fill and submit flow
- [ ] T130 [US2] Test submission retrieval and display
- [ ] T131 [US2] Verify all US2 tests pass

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently. Users can create templates AND fill/submit forms.

---

## Phase 5: User Story 3 - Comprehensive Control Library (Priority: P3)

**Goal**: Expand control library with advanced controls (file upload, rich text, rating, multi-select, date range, etc.)

**Independent Test**: Verify each control type can be added to template, configured, used in data entry, and correctly saves/retrieves data

### Tests for User Story 3 (Write First, Verify Failure)

- [ ] T132 [P] [US3] Component test for FileUpload in tests/FormR.Web.Tests/renderer/controls/FileUpload.test.tsx
- [ ] T133 [P] [US3] Component test for RichTextEditor in tests/FormR.Web.Tests/renderer/controls/RichTextEditor.test.tsx
- [ ] T134 [P] [US3] Component test for RatingScale in tests/FormR.Web.Tests/renderer/controls/RatingScale.test.tsx
- [ ] T135 [P] [US3] Component test for MultiSelect in tests/FormR.Web.Tests/renderer/controls/MultiSelect.test.tsx
- [ ] T136 [P] [US3] Component test for DateRangePicker in tests/FormR.Web.Tests/renderer/controls/DateRangePicker.test.tsx
- [ ] T137 [P] [US3] Unit test for file upload validation in tests/FormR.API.Tests/Services/FileUploadServiceTests.cs
- [ ] T138 [US3] E2E test for advanced controls in tests/FormR.Web.Tests/e2e/advanced-controls.spec.ts

### Backend Services for User Story 3

- [ ] T139 [US3] Implement FileUploadService in src/FormR.API/Services/FileUploadService.cs
- [ ] T140 [US3] Configure blob storage provider (Azure/AWS/local) in src/FormR.API/Services/BlobStorageService.cs

### Backend API Endpoints for User Story 3

- [ ] T141 [US3] Implement file upload endpoint (POST /v1/uploads) in src/FormR.API/Controllers/UploadsController.cs

### Frontend Control Library Expansion for User Story 3

- [ ] T142 [P] [US3] Create MultilineText control in src/FormR.Web/src/renderer/controls/MultilineText.tsx
- [ ] T143 [P] [US3] Create EmailInput control in src/FormR.Web/src/renderer/controls/EmailInput.tsx
- [ ] T144 [P] [US3] Create PhoneInput control in src/FormR.Web/src/renderer/controls/PhoneInput.tsx
- [ ] T145 [P] [US3] Create MultiSelect control in src/FormR.Web/src/renderer/controls/MultiSelect.tsx
- [ ] T146 [P] [US3] Create TimePicker control in src/FormR.Web/src/renderer/controls/TimePicker.tsx
- [ ] T147 [P] [US3] Create DateRangePicker control in src/FormR.Web/src/renderer/controls/DateRangePicker.tsx
- [ ] T148 [P] [US3] Create FileUpload control in src/FormR.Web/src/renderer/controls/FileUpload.tsx
- [ ] T149 [P] [US3] Create RichTextEditor control (integrate library) in src/FormR.Web/src/renderer/controls/RichTextEditor.tsx
- [ ] T150 [P] [US3] Create RatingScale control in src/FormR.Web/src/renderer/controls/RatingScale.tsx
- [ ] T151 [P] [US3] Create Slider control in src/FormR.Web/src/renderer/controls/Slider.tsx
- [ ] T152 [P] [US3] Create SignaturePad control in src/FormR.Web/src/renderer/controls/SignaturePad.tsx

### Frontend Layout Controls for User Story 3

- [ ] T153 [P] [US3] Create Section layout control in src/FormR.Web/src/renderer/controls/Section.tsx
- [ ] T154 [P] [US3] Create ColumnContainer layout control in src/FormR.Web/src/renderer/controls/ColumnContainer.tsx
- [ ] T155 [P] [US3] Create TabPanel layout control in src/FormR.Web/src/renderer/controls/TabPanel.tsx
- [ ] T156 [P] [US3] Create Accordion layout control in src/FormR.Web/src/renderer/controls/Accordion.tsx

### Frontend Conditional Logic for User Story 3

- [ ] T157 [US3] Implement conditional field visibility engine in src/FormR.Web/src/renderer/ConditionalLogic.ts
- [ ] T158 [US3] Add conditional rules editor to properties panel in src/FormR.Web/src/designer/properties-panel/ConditionalRulesEditor.tsx

### Control Library Metadata for User Story 3

- [ ] T159 [US3] Update ControlLibrary seed data with all control types in src/FormR.Data/Migrations/
- [ ] T160 [US3] Update control palette UI with new categories

### Integration for User Story 3

- [ ] T161 [US3] Test all advanced controls in designer
- [ ] T162 [US3] Test all advanced controls in form renderer
- [ ] T163 [US3] Test conditional field visibility
- [ ] T164 [US3] Test file uploads end-to-end
- [ ] T165 [US3] Verify all US3 tests pass

**Checkpoint**: All control types available. Users can build complex forms with advanced features.

---

## Phase 6: User Story 4 - Template Management and Versioning (Priority: P4)

**Goal**: Enable template organization (folders), duplication, editing with automatic versioning, and deletion protection

**Independent Test**: Create folders, organize templates, duplicate template, edit template with submissions (verify versioning), attempt deletion of template with submissions (verify protection)

### Tests for User Story 4 (Write First, Verify Failure)

- [ ] T166 [P] [US4] Unit test for template versioning logic in tests/FormR.Core.Tests/Services/TemplateVersioningTests.cs
- [ ] T167 [P] [US4] Unit test for TemplateFolder model in tests/FormR.Core.Tests/Models/TemplateFolderTests.cs
- [ ] T168 [P] [US4] Contract test for PUT /v1/templates/{id} (versioning) in tests/FormR.API.Tests/Controllers/TemplatesControllerTests.cs
- [ ] T169 [P] [US4] Contract test for POST /v1/templates/{id}/duplicate in tests/FormR.API.Tests/Controllers/TemplatesControllerTests.cs
- [ ] T170 [P] [US4] Contract test for DELETE /v1/templates/{id} in tests/FormR.API.Tests/Controllers/TemplatesControllerTests.cs
- [ ] T171 [US4] E2E test for template versioning flow in tests/FormR.Web.Tests/e2e/template-versioning.spec.ts

### Backend Models for User Story 4

- [ ] T172 [P] [US4] Create TemplateFolder model in src/FormR.Core/Models/TemplateFolder.cs
- [ ] T173 [P] [US4] Create TemplateFolderMapping model in src/FormR.Core/Models/TemplateFolderMapping.cs

### Database Configuration for User Story 4

- [ ] T174 [US4] Configure TemplateFolder entity mapping in src/FormR.Data/FormBuilderContext.cs
- [ ] T175 [US4] Configure TemplateFolderMapping entity mapping in src/FormR.Data/FormBuilderContext.cs
- [ ] T176 [US4] Create migration for folder tables in src/FormR.Data/Migrations/

### Backend Services for User Story 4

- [ ] T177 [US4] Implement template versioning logic in TemplateService
- [ ] T178 [US4] Implement template duplication logic in TemplateService
- [ ] T179 [US4] Implement FolderService in src/FormR.API/Services/FolderService.cs

### Backend API Endpoints for User Story 4

- [ ] T180 [US4] Implement TemplatesController.Update with versioning (PUT /v1/templates/{id}) in src/FormR.API/Controllers/TemplatesController.cs
- [ ] T181 [US4] Implement TemplatesController.Duplicate (POST /v1/templates/{id}/duplicate) in src/FormR.API/Controllers/TemplatesController.cs
- [ ] T182 [US4] Implement TemplatesController.Delete with protection (DELETE /v1/templates/{id}) in src/FormR.API/Controllers/TemplatesController.cs
- [ ] T183 [P] [US4] Implement FoldersController in src/FormR.API/Controllers/FoldersController.cs

### Frontend Services for User Story 4

- [ ] T184 [P] [US4] Create FolderService API client in src/FormR.Web/src/services/folder-service.ts

### Frontend Components for User Story 4

- [ ] T185 [P] [US4] Create FolderTree component in src/FormR.Web/src/components/FolderTree.tsx
- [ ] T186 [P] [US4] Create TemplateVersionHistory component in src/FormR.Web/src/components/TemplateVersionHistory.tsx
- [ ] T187 [P] [US4] Create DuplicateTemplateDialog in src/FormR.Web/src/components/DuplicateTemplateDialog.tsx
- [ ] T188 [P] [US4] Create DeleteConfirmationDialog in src/FormR.Web/src/components/DeleteConfirmationDialog.tsx

### Frontend Integration for User Story 4

- [ ] T189 [US4] Add folder management UI to TemplateListPage
- [ ] T190 [US4] Add duplicate button to template actions
- [ ] T191 [US4] Add version history viewer
- [ ] T192 [US4] Add delete protection logic
- [ ] T193 [US4] Implement template update with versioning detection
- [ ] T194 [US4] Test complete folder and versioning flow
- [ ] T195 [US4] Verify all US4 tests pass

**Checkpoint**: All user stories should now be independently functional. Complete template management system.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

### Performance Optimization

- [ ] T196 [P] Add response caching for control library in src/FormR.API/Controllers/ControlsController.cs
- [ ] T197 [P] Add template list caching in src/FormR.API/Services/TemplateService.cs
- [ ] T198 [P] Optimize database queries with eager loading in repositories
- [ ] T199 [P] Add frontend lazy loading for routes in src/FormR.Web/src/App.tsx
- [ ] T200 [P] Implement virtualization for template list in src/FormR.Web/src/pages/TemplateListPage.tsx

### Security Hardening

- [ ] T201 [P] Add input sanitization middleware in src/FormR.API/Middleware/SanitizationMiddleware.cs
- [ ] T202 [P] Configure content security policy headers in src/FormR.API/Program.cs
- [ ] T203 [P] Add CSRF protection for state-changing operations
- [ ] T204 [P] Implement rate limiting per endpoint in src/FormR.API/Program.cs
- [ ] T205 [P] Add XSS protection in frontend form renderer

### Accessibility (WCAG 2.1 AA)

- [ ] T206 [P] Add ARIA labels to all designer controls
- [ ] T207 [P] Implement keyboard navigation for canvas
- [ ] T208 [P] Add focus management for properties panel
- [ ] T209 [P] Test with screen readers (NVDA, JAWS)
- [ ] T210 [P] Ensure color contrast ratios meet AA standards

### Documentation

- [ ] T211 [P] Update README.md with setup instructions
- [ ] T212 [P] Create API documentation from OpenAPI spec
- [ ] T213 [P] Document control library metadata in docs/controls.md
- [ ] T214 [P] Create user guide for template designer in docs/user-guide.md
- [ ] T215 [P] Document versioning system in docs/versioning.md

### NuGet Packaging

- [ ] T216 [P] Configure NuGet package metadata for FormR.Core
- [ ] T217 [P] Configure NuGet package metadata for FormR.Data
- [ ] T218 [P] Create package README files
- [ ] T219 [P] Test NuGet package installation in sample project
- [ ] T220 Build and publish NuGet packages

### Quality Assurance

- [ ] T221 Run full test suite (backend + frontend)
- [ ] T222 Verify code coverage meets 80% threshold
- [ ] T223 Run load tests (100+ concurrent users per plan.md)
- [ ] T224 Test template load performance (<2s for 50 controls)
- [ ] T225 Test template save performance (<1s for 50 controls)
- [ ] T226 Test validation feedback latency (<100ms)
- [ ] T227 Validate quickstart.md steps work end-to-end
- [ ] T228 Cross-browser testing (Chrome, Firefox, Safari, Edge - last 2 versions)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-6)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3 → P4)
- **Polish (Phase 7)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories ✅ INDEPENDENT
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - Requires templates from US1 but independently testable ✅ INDEPENDENT
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - Extends US1 designer and US2 renderer but independently testable ✅ INDEPENDENT
- **User Story 4 (P4)**: Can start after Foundational (Phase 2) - Extends US1 template management but independently testable ✅ INDEPENDENT

### Within Each User Story

- Tests MUST be written and verified to FAIL before implementation
- Models before services
- Services before endpoints
- Core implementation before integration
- Story complete and tested before moving to next priority

### Parallel Opportunities

- **Setup (Phase 1)**: All tasks marked [P] can run in parallel (T002-T017)
- **Foundational (Phase 2)**: Tasks marked [P] can run in parallel within groups (T018-T020 enums, T025-T027 interfaces, etc.)
- **Once Foundational completes**: All user stories can start in parallel if team capacity allows
- **Within each story**: All tests marked [P] can run in parallel, all models marked [P] can run in parallel
- **Polish (Phase 7)**: All tasks marked [P] can run in parallel

---

## Parallel Example: User Story 1

```bash
# Launch all unit tests for User Story 1 together (write first, verify failure):
Task T037: "Unit test for FormTemplate validation in tests/FormR.Core.Tests/Models/FormTemplateTests.cs"
Task T038: "Unit test for FormControl validation in tests/FormR.Core.Tests/Models/FormControlTests.cs"
Task T039: "Unit test for ControlLibrary seeding in tests/FormR.Data.Tests/SeedDataTests.cs"

# Launch all contract tests for User Story 1 together:
Task T040: "Contract test for POST /v1/templates in tests/FormR.API.Tests/Controllers/TemplatesControllerTests.cs"
Task T041: "Contract test for GET /v1/templates in tests/FormR.API.Tests/Controllers/TemplatesControllerTests.cs"
Task T042: "Contract test for GET /v1/templates/{id} in tests/FormR.API.Tests/Controllers/TemplatesControllerTests.cs"
Task T043: "Contract test for GET /v1/controls in tests/FormR.API.Tests/Controllers/ControlsControllerTests.cs"

# Launch all models for User Story 1 together:
Task T048: "Create FormTemplate model in src/FormR.Core/Models/FormTemplate.cs"
Task T049: "Create FormControl model in src/FormR.Core/Models/FormControl.cs"
Task T050: "Create ControlLibrary model in src/FormR.Core/Models/ControlLibrary.cs"

# Launch all validators for User Story 1 together:
Task T051: "Create TemplateValidator using FluentValidation in src/FormR.Core/Validation/TemplateValidator.cs"
Task T052: "Create ControlValidator using FluentValidation in src/FormR.Core/Validation/ControlValidator.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T017)
2. Complete Phase 2: Foundational (T018-T036) - CRITICAL - blocks all stories
3. Complete Phase 3: User Story 1 (T037-T086) - Write tests first, verify failure, then implement
4. **STOP and VALIDATE**: Test User Story 1 independently - can create, configure, save, load templates
5. Deploy/demo MVP if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP: Template Designer!)
3. Add User Story 2 → Test independently → Deploy/Demo (Form Fill & Submit!)
4. Add User Story 3 → Test independently → Deploy/Demo (Advanced Controls!)
5. Add User Story 4 → Test independently → Deploy/Demo (Full Template Management!)
6. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together (T001-T036)
2. Once Foundational is done:
   - Developer A: User Story 1 (T037-T086)
   - Developer B: User Story 2 (T087-T131)
   - Developer C: User Story 3 (T132-T165)
   - Developer D: User Story 4 (T166-T195)
3. Stories complete and integrate independently
4. Team collaborates on Polish phase

---

## Task Summary

**Total Tasks**: 228
- **Phase 1 (Setup)**: 17 tasks
- **Phase 2 (Foundational)**: 19 tasks (CRITICAL - blocks all stories)
- **Phase 3 (User Story 1 - P1)**: 50 tasks (Template Designer MVP)
- **Phase 4 (User Story 2 - P2)**: 45 tasks (Form Fill & Submit)
- **Phase 5 (User Story 3 - P3)**: 34 tasks (Advanced Controls)
- **Phase 6 (User Story 4 - P4)**: 30 tasks (Template Management)
- **Phase 7 (Polish)**: 33 tasks

**Parallel Tasks**: 147 tasks marked [P] can run in parallel with other [P] tasks in same phase

**Independent Stories**: All 4 user stories are independently testable and deliverable

**MVP Scope**: Phase 1 + Phase 2 + Phase 3 = 86 tasks for template designer MVP

---

## Notes

- **[P] tasks** = different files, no dependencies within phase
- **[Story] label** maps task to specific user story for traceability
- **Each user story** should be independently completable and testable
- **Verify tests fail** before implementing (TDD per constitution)
- **Commit after each task** or logical group with issue reference
- **Stop at any checkpoint** to validate story independently
- **Constitution compliance**: All tasks reference #1, tests written first, commits will reference issue
- **Avoid**: vague tasks, same file conflicts, cross-story dependencies that break independence

---

**Ready for Implementation**: Follow GitHub development workflow per constitution - create sub-tasks as issues if needed, commit with #1 reference, run tests first.
