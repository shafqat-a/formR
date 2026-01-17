# QuickStart Guide: FormR Implementation

**Date**: 2026-01-18
**Phase**: 1 - Design
**Related**: [plan.md](./plan.md), [data-model.md](./data-model.md), [contracts/api-spec.yaml](./contracts/api-spec.yaml)

## Overview

This guide provides developers with the essential steps to set up and begin implementing the FormR form builder system.

## Prerequisites

- .NET 10 SDK
- Node.js 20+ (for frontend)
- PostgreSQL 15+ (or Docker)
- Git
- IDE: Visual Studio 2022, VS Code, or Rider

## Project Setup

### 1. Initialize .NET Solution

```bash
# Create solution structure
mkdir FormR
cd FormR

# Create solution
dotnet new sln -n FormR

# Create projects
dotnet new classlib -n FormR.Core -f net10.0
dotnet new classlib -n FormR.Data -f net10.0
dotnet new webapi -n FormR.API -f net10.0

# Add to solution
dotnet sln add FormR.Core/FormR.Core.csproj
dotnet sln add FormR.Data/FormR.Data.csproj
dotnet sln add FormR.API/FormR.API.csproj

# Create test projects
dotnet new xunit -n FormR.Core.Tests
dotnet new xunit -n FormR.Data.Tests
dotnet new xunit -n FormR.API.Tests

dotnet sln add FormR.Core.Tests/FormR.Core.Tests.csproj
dotnet sln add FormR.Data.Tests/FormR.Data.Tests.csproj
dotnet sln add FormR.API.Tests/FormR.API.Tests.csproj

# Add project references
dotnet add FormR.Data/FormR.Data.csproj reference FormR.Core/FormR.Core.csproj
dotnet add FormR.API/FormR.API.csproj reference FormR.Core/FormR.Core.csproj
dotnet add FormR.API/FormR.API.csproj reference FormR.Data/FormR.Data.csproj
```

### 2. Install NuGet Packages

```bash
# FormR.Core
cd FormR.Core
dotnet add package FluentValidation
dotnet add package System.Text.Json

# FormR.Data
cd ../FormR.Data
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design

# FormR.API
cd ../FormR.API
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package AspNetCoreRateLimit

# Test projects
cd ../FormR.Core.Tests
dotnet add package FluentAssertions
dotnet add package xUnit
cd ../FormR.Data.Tests
dotnet add package Testcontainers.PostgreSql
dotnet add package FluentAssertions
cd ../FormR.API.Tests
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package FluentAssertions
```

### 3. Initialize Frontend

```bash
# In repository root, create frontend
npm create vite@latest FormR.Web -- --template react-ts
cd FormR.Web

# Install dependencies
npm install
npm install @dnd-kit/core @dnd-kit/sortable @dnd-kit/modifiers
npm install react-grid-layout
npm install axios
npm install -D vitest @testing-library/react @testing-library/jest-dom
npm install -D playwright @playwright/test
```

### 4. Database Setup

```bash
# Using Docker
docker run --name formr-postgres -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres:15

# Or install PostgreSQL locally
# Connection string: "Host=localhost;Database=formr;Username=postgres;Password=password"
```

## Implementation Roadmap

### Phase 1: Core Domain Models (P1 - Week 1)

**Priority**: Implement FormR.Core models

1. Create entity classes in `FormR.Core/Models/`
   - FormTemplate.cs
   - FormControl.cs
   - FormInstance.cs (with SubmissionData JSON property)
   - Enums (ControlType, InstanceStatus)

2. Define interfaces in `FormR.Core/Interfaces/`
   - IFormRepository.cs
   - ITemplateService.cs
   - IValidationEngine.cs

3. Implement validators in `FormR.Core/Validation/`
   - TemplateValidator.cs (using FluentValidation)
   - SubmissionValidator.cs (validates JSON submission data against template)

**Note**: Form submission data is stored as JSON in FormInstance.SubmissionData property. No separate FormSubmission table exists. This simplifies queries and provides schema flexibility.

**Test Coverage**: Unit tests for all validators

### Phase 2: Data Layer with Provider Pattern (P1 - Week 1-2)

**Priority**: Implement FormR.Data with PostgreSQL

1. Create `FormBuilderContext.cs` (DbContext)
2. Implement `IDataProvider` interface
3. Create `PostgreSqlProvider.cs`
4. Configure entity mappings (Fluent API)
   - FormInstance.SubmissionData → JSONB column (PostgreSQL)
   - Enable JSON querying and indexing
5. Generate initial migration
6. Seed ControlLibrary data

**Example**: FormInstance entity mapping with JSON storage:
```csharp
modelBuilder.Entity<FormInstance>(entity =>
{
    entity.Property(e => e.SubmissionData)
          .HasColumnType("jsonb")  // PostgreSQL JSONB for indexing
          .IsRequired(false);       // Null until submitted
});
```

**Test Coverage**: Integration tests using Testcontainers

### Phase 3: API Layer (P1 - Week 2)

**Priority**: Implement FormR.API controllers

1. Templates API (`TemplatesController.cs`)
   - GET /templates (list)
   - POST /templates (create)
   - GET /templates/{id} (get)
   - PUT /templates/{id} (update with versioning)
   - DELETE /templates/{id} (soft delete)
   - POST /templates/{id}/duplicate

2. Forms API (`FormsController.cs`)
   - POST /forms (instantiate template)
   - GET /forms/{id} (get instance)
   - POST /forms/{id}/submit (submit data)

3. Submissions API (`SubmissionsController.cs`)
   - GET /submissions (list)
   - GET /submissions/{id} (get detail)

4. Controls API (`ControlsController.cs`)
   - GET /controls (library)

**Test Coverage**: Contract tests verifying OpenAPI spec compliance

### Phase 4: Frontend Designer (P1 - Week 3-4)

**Priority**: Build React visual designer

1. **Canvas Component** (`src/designer/canvas/`)
   - DnD integration with dnd-kit
   - Snap-to-grid (8px)
   - Visual guides and alignment

2. **Control Palette** (`src/designer/controls/`)
   - Categorized control list
   - Draggable controls

3. **Properties Panel** (`src/designer/properties-panel/`)
   - Context-sensitive property editors
   - Validation rule configuration

4. **State Management**
   - Template state (Zustand or Redux Toolkit)
   - Canvas interaction state

**Test Coverage**: Component tests with Vitest + Testing Library

### Phase 5: Form Rendering & Submission (P2 - Week 5)

**Priority**: Implement form instance rendering

1. **Renderer Component** (`src/renderer/`)
   - Dynamic form generation from template
   - Real-time validation
   - Data binding

2. **Submission Flow**
   - Form instance creation
   - Draft saving (auto-save)
   - Final submission

**Test Coverage**: E2E tests with Playwright

### Phase 6: Control Library Expansion (P3 - Week 6-8)

**Priority**: Implement 15+ control types

Implement in order:
1. Basic: TextInput, MultilineText, NumberInput, EmailInput
2. Selection: Dropdown, Checkbox, RadioGroup, MultiSelect
3. DateTime: DatePicker, TimePicker, DateRangePicker
4. Advanced: FileUpload, RichTextEditor, RatingScale, Slider
5. Layout: Section, ColumnContainer

**Test Coverage**: Unit tests for each control + integration tests

### Phase 7: Template Management (P4 - Week 9-10)

**Priority**: Folders, organization, versioning UI

1. Folder management UI
2. Template search and filtering
3. Version history viewer
4. Bulk operations

**Test Coverage**: E2E tests for workflows

## Development Workflow

### Test-First Approach (per Constitution)

```bash
# 1. Write failing test
# FormR.Core.Tests/Models/FormTemplateTests.cs
[Fact]
public void Template_ShouldRequireName()
{
    var template = new FormTemplate { Name = null };
    var validator = new TemplateValidator();

    var result = validator.Validate(template);

    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "Name");
}

# 2. Run test (should fail)
dotnet test

# 3. Implement
// FormR.Core/Models/FormTemplate.cs
public class FormTemplate
{
    public string Name { get; set; } = string.Empty; // Required
}

# 4. Run test (should pass)
dotnet test
```

### GitHub Workflow (per Constitution)

All work follows issue #1: https://github.com/shafqat-a/formR/issues/1

```bash
# Create sub-tasks as needed
gh issue create --title "Implement FormTemplate entity" --body "Part of #1" --label "task"

# Commit with issue reference
git commit -m "feat: implement FormTemplate entity model

Relates to #1

Co-Authored-By: Claude Sonnet 4.5 <noreply@anthropic.com>"

# Push and create PR
git push
gh pr create --title "Add form template core models" --body "Resolves #1 (partial)"
```

## Configuration Files

### appsettings.json (FormR.API)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=formr;Username=postgres;Password=password"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-256-bits",
    "Issuer": "FormR.API",
    "Audience": "FormR.Client",
    "ExpirationMinutes": 60
  },
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      }
    ]
  }
}
```

### vite.config.ts (FormR.Web)

```typescript
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, '/v1')
      }
    }
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/test/setup.ts'
  }
})
```

## Running the Application

```bash
# Terminal 1: API
cd FormR.API
dotnet run

# Terminal 2: Frontend
cd FormR.Web
npm run dev

# Access application
# Frontend: http://localhost:3000
# API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

## Next Steps

1. Run `/speckit.tasks` to generate detailed task list
2. Create first test following TDD approach
3. Implement Phase 1 (Core Domain Models)
4. Set up CI/CD pipeline (GitHub Actions)
5. Configure code quality tools (SonarQube, ESLint)

## Resources

- [OpenAPI Spec](./contracts/api-spec.yaml)
- [Data Model Documentation](./data-model.md)
- [Research & Technology Decisions](./research.md)
- [GitHub Issue #1](https://github.com/shafqat-a/formR/issues/1)
