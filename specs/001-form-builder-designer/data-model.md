# Data Model: Form Builder with Visual Designer

**Date**: 2026-01-18
**Phase**: 1 - Design
**Related**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md)

## Overview

This document defines the data model for the form builder system. The model supports form template creation, versioning, instantiation, and data submission with database-agnostic persistence through Entity Framework Core's provider pattern.

## Entity Relationship Diagram

```
┌─────────────────────┐
│   FormTemplate      │
│──────────────────── │
│ Id (Guid)           │───┐
│ Name (string)       │   │
│ Description (string)│   │
│ Version (int)       │   │
│ BaseTemplateId (?)  │←──┘ (self-reference for versioning)
│ CreatedDate         │
│ ModifiedDate        │
│ IsDeleted (bool)    │
│ TenantId (Guid)     │
└─────────────────────┘
         │ 1
         │
         │ *
┌─────────────────────┐
│    FormControl      │
│──────────────────── │
│ Id (Guid)           │
│ TemplateId (Guid)   │───→ FormTemplate.Id
│ Type (enum)         │
│ Label (string)      │
│ Placeholder (string)│
│ DefaultValue (str)  │
│ IsRequired (bool)   │
│ ValidationRules(JSON│
│ Position (JSON)     │  {x, y, width, height, zIndex}
│ Properties (JSON)   │  control-specific config
│ ParentControlId (?) │  for nested containers
│ Order (int)         │
└─────────────────────┘

┌─────────────────────┐
│   FormInstance      │
│──────────────────── │
│ Id (Guid)           │
│ TemplateId (Guid)   │───→ FormTemplate.Id
│ TemplateVersion(int)│  preserves version at instantiation
│ Status (enum)       │  Draft, Submitted
│ SubmissionData(JSON)│  all form field values as JSON
│ CreatedDate         │
│ SubmittedDate (?)   │
│ SubmittedBy (string)│
│ TenantId (Guid)     │
└─────────────────────┘

┌─────────────────────┐
│  TemplateFolder     │   (Optional - P4)
│──────────────────── │
│ Id (Guid)           │
│ Name (string)       │
│ ParentFolderId (?)  │  for nested folders
│ TenantId (Guid)     │
│ CreatedDate         │
└─────────────────────┘
         │ *
         │
         │ *
┌─────────────────────┐
│TemplateFolderMapping│
│──────────────────── │
│ TemplateId (Guid)   │───→ FormTemplate.Id
│ FolderId (Guid)     │───→ TemplateFolder.Id
└─────────────────────┘

┌─────────────────────┐
│   ControlLibrary    │   (Reference data)
│──────────────────── │
│ Type (string/PK)    │  "TextInput", "Dropdown", etc.
│ Category (enum)     │  Basic, Selection, Advanced, Layout
│ Icon (string)       │  UI icon identifier
│ ConfigSchema (JSON) │  available properties
│ DefaultProps (JSON) │  default configuration
└─────────────────────┘
```

## Core Entities

### 1. FormTemplate

Represents a reusable form design created in the visual designer.

**Purpose**: Stores the complete form structure including all controls, layout, and configuration. Supports versioning to preserve historical submission integrity.

**Attributes**:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Id | Guid | Yes | Primary key |
| Name | string(200) | Yes | Template display name |
| Description | string(1000) | No | Template purpose/notes |
| Version | int | Yes | Version number (increments on edit with submissions) |
| BaseTemplateId | Guid? | No | Points to original template when versioned (null for v1) |
| CreatedDate | DateTime | Yes | UTC timestamp of creation |
| ModifiedDate | DateTime | Yes | UTC timestamp of last modification |
| IsDeleted | bool | Yes | Soft delete flag (default: false) |
| TenantId | Guid | Yes | Multi-tenancy isolation |

**Relationships**:
- One-to-Many with FormControl (cascade delete)
- One-to-Many with FormInstance (no cascade)
- Self-referencing for versioning (BaseTemplateId → Id)
- Many-to-Many with TemplateFolder via TemplateFolderMapping

**Versioning Logic**:
- When a template with existing submissions is edited, a new template is created:
  - New Id generated
  - Version = BaseTemplate.Version + 1
  - BaseTemplateId = original template Id
  - All controls copied with new Ids
- Original template remains unchanged, preserving submission links

**Indexes**:
- Primary: Id
- Unique: (Name, Version, TenantId) - prevents duplicate versions within tenant
- Index: TenantId, BaseTemplateId, IsDeleted

---

### 2. FormControl

Represents an individual input element within a form template.

**Purpose**: Defines control type, properties, position, validation rules, and relationships within the template.

**Attributes**:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Id | Guid | Yes | Primary key |
| TemplateId | Guid | Yes | Foreign key to FormTemplate |
| Type | ControlType (enum) | Yes | Control type (see enum below) |
| Label | string(200) | Yes | Display label |
| Placeholder | string(200) | No | Placeholder text for inputs |
| DefaultValue | string | No | Pre-filled value |
| IsRequired | bool | Yes | Validation: required field (default: false) |
| ValidationRules | JSON | No | Validation config (regex, min/max, custom) |
| Position | JSON | Yes | `{x, y, width, height, zIndex}` canvas coords |
| Properties | JSON | No | Control-specific configuration |
| ParentControlId | Guid? | No | For nested controls (e.g., tab panel children) |
| Order | int | Yes | Tab order / rendering sequence |

**ControlType Enum**:
```csharp
public enum ControlType
{
    // Basic Inputs
    TextInput,
    MultilineText,
    NumberInput,
    EmailInput,
    PhoneInput,

    // Selection
    Dropdown,
    Checkbox,
    RadioGroup,
    MultiSelect,

    // Date/Time
    DatePicker,
    TimePicker,
    DateRangePicker,

    // Advanced
    FileUpload,
    RichTextEditor,
    RatingScale,
    Slider,
    SignaturePad,

    // Layout
    Section,
    ColumnContainer,
    TabPanel,
    Accordion
}
```

**Validation Rules JSON Schema**:
```json
{
  "type": "object",
  "properties": {
    "regex": {"type": "string"},
    "min": {"type": "number"},
    "max": {"type": "number"},
    "minLength": {"type": "integer"},
    "maxLength": {"type": "integer"},
    "allowedExtensions": {"type": "array", "items": {"type": "string"}},
    "maxFileSize": {"type": "integer"},
    "customValidation": {"type": "string"}
  }
}
```

**Position JSON Schema**:
```json
{
  "type": "object",
  "required": ["x", "y"],
  "properties": {
    "x": {"type": "integer"},
    "y": {"type": "integer"},
    "width": {"type": "integer"},
    "height": {"type": "integer"},
    "zIndex": {"type": "integer", "default": 0}
  }
}
```

**Properties JSON** (control-specific examples):
```json
// TextInput
{
  "maxLength": 100,
  "autocomplete": "name",
  "spellcheck": true
}

// Dropdown
{
  "options": [
    {"value": "option1", "label": "Option 1"},
    {"value": "option2", "label": "Option 2"}
  ],
  "allowCustom": false
}

// FileUpload
{
  "multiple": true,
  "maxFiles": 5,
  "allowedTypes": ["image/*", ".pdf"],
  "maxSizeBytes": 10485760
}

// ConditionalField
{
  "showWhen": {
    "controlId": "guid-of-other-control",
    "operator": "equals",
    "value": "specific-value"
  }
}
```

**Relationships**:
- Many-to-One with FormTemplate (required)
- Self-referencing for nesting (ParentControlId → Id)
- One-to-Many with FormSubmission (no cascade)

**Indexes**:
- Primary: Id
- Foreign: TemplateId
- Index: ParentControlId, Order

---

### 3. FormInstance

Represents a specific occurrence of a form being filled out based on a template.

**Purpose**: Links submitted data to a specific template version, tracks submission status and metadata. Stores all form field data as JSON for flexibility and simplicity.

**Attributes**:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Id | Guid | Yes | Primary key |
| TemplateId | Guid | Yes | Foreign key to FormTemplate |
| TemplateVersion | int | Yes | Snapshot of template version at instantiation |
| Status | InstanceStatus (enum) | Yes | Draft or Submitted |
| SubmissionData | JSON | No | All form field values stored as JSON object (null until data entered) |
| CreatedDate | DateTime | Yes | UTC timestamp of instance creation |
| SubmittedDate | DateTime? | No | UTC timestamp of submission (null if draft) |
| SubmittedBy | string(200) | No | User identifier (email, username, etc.) |
| TenantId | Guid | Yes | Multi-tenancy isolation |

**InstanceStatus Enum**:
```csharp
public enum InstanceStatus
{
    Draft = 0,      // In progress, can be edited
    Submitted = 1   // Finalized, read-only
}
```

**SubmissionData JSON Schema**:

The SubmissionData field stores all form field values as a single JSON object, with control IDs as keys:

```json
{
  "controlId-1": {
    "value": "John Doe",
    "type": "string"
  },
  "controlId-2": {
    "value": 42,
    "type": "number"
  },
  "controlId-3": {
    "value": "2026-01-18T10:30:00Z",
    "type": "date"
  },
  "controlId-4": {
    "value": true,
    "type": "boolean"
  },
  "controlId-5": {
    "value": {
      "fileName": "document.pdf",
      "fileUrl": "blob://storage/abc123",
      "fileSizeBytes": 102400,
      "mimeType": "application/pdf"
    },
    "type": "file"
  },
  "controlId-6": {
    "value": ["option1", "option2", "option3"],
    "type": "array"
  }
}
```

**Supported Value Types**:
- **string**: Text, email, phone, etc.
- **number**: Integer or decimal values
- **date**: ISO 8601 date-time strings
- **boolean**: true/false for checkboxes
- **file**: Object with fileName, fileUrl, fileSizeBytes, mimeType
- **array**: Multiple values for multi-select, checkboxes
- **object**: Complex nested data for advanced controls

**Relationships**:
- Many-to-One with FormTemplate (required, no cascade)

**Indexes**:
- Primary: Id
- Foreign: TemplateId
- Index: TenantId, Status, SubmittedDate, SubmittedBy
- JSONB Index: SubmissionData (for PostgreSQL queries)

**Business Rules**:
- TemplateVersion is immutable after creation (preserves link to template version)
- Status can only transition: Draft → Submitted (no rollback)
- SubmittedDate set automatically on status change to Submitted
- SubmissionData is nullable (null until user starts entering data)
- SubmissionData is immutable after Status = Submitted
- Draft instances can be deleted without restriction
- Submitted instances require authorization to delete
- Validation performed against template controls before saving

---

## Supporting Entities

### 5. TemplateFolder (Priority P4)

Represents an organizational container for grouping related form templates.

**Purpose**: Enables hierarchical organization of templates (folders/sub

folders).

**Attributes**:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Id | Guid | Yes | Primary key |
| Name | string(200) | Yes | Folder name |
| ParentFolderId | Guid? | No | Parent folder for nesting (null = root) |
| TenantId | Guid | Yes | Multi-tenancy isolation |
| CreatedDate | DateTime | Yes | UTC timestamp of creation |

**Relationships**:
- Self-referencing for nesting (ParentFolderId → Id)
- Many-to-Many with FormTemplate via TemplateFolderMapping

**Indexes**:
- Primary: Id
- Index: ParentFolderId, TenantId

**Business Rules**:
- Maximum nesting depth: 5 levels (prevent infinite recursion)
- Cannot delete folder with children (must move or delete children first)
- Templates can belong to multiple folders (tagging system)

---

### 6. TemplateFolderMapping (Priority P4)

Junction table for many-to-many relationship between templates and folders.

**Attributes**:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| TemplateId | Guid | Yes | Foreign key to FormTemplate |
| FolderId | Guid | Yes | Foreign key to TemplateFolder |

**Relationships**:
- Many-to-One with FormTemplate
- Many-to-One with TemplateFolder

**Indexes**:
- Primary: (TemplateId, FolderId) composite key

---

### 7. ControlLibrary (Reference Data)

Defines available control types and their configurations.

**Purpose**: Metadata for control palette in designer. Read-only reference data.

**Attributes**:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Type | string(50) | Yes | Primary key, matches ControlType enum |
| Category | ControlCategory (enum) | Yes | Palette category |
| Icon | string(50) | Yes | Icon identifier for UI |
| ConfigSchema | JSON | Yes | JSON Schema defining available properties |
| DefaultProps | JSON | Yes | Default configuration when added to canvas |

**ControlCategory Enum**:
```csharp
public enum ControlCategory
{
    BasicInputs,
    SelectionControls,
    DateTimeControls,
    AdvancedControls,
    LayoutControls,
    SpecialControls
}
```

**Example Record**:
```json
{
  "Type": "TextInput",
  "Category": "BasicInputs",
  "Icon": "text-fields",
  "ConfigSchema": {
    "type": "object",
    "properties": {
      "maxLength": {"type": "integer", "minimum": 1},
      "autocomplete": {"type": "string"},
      "spellcheck": {"type": "boolean"}
    }
  },
  "DefaultProps": {
    "maxLength": 100,
    "spellcheck": true
  }
}
```

**Data Loading**: Seed data loaded via EF Core migrations.

---

## Database Provider Pattern

### Provider Interface

```csharp
public interface IDataProvider
{
    DbContext CreateContext();
    void ConfigureMigrations(DbContextOptionsBuilder options);
    string GetProviderName();
}
```

### Supported Providers

1. **PostgreSqlProvider** (default)
   - Uses Npgsql.EntityFrameworkCore.PostgreSQL
   - Optimized for JSON column types (JSONB)
   - Supports full-text search on template names/descriptions

2. **SqlServerProvider** (future)
   - Uses Microsoft.EntityFrameworkCore.SqlServer
   - Uses NVARCHAR(MAX) for JSON storage
   - Compatibility mode for JSON operations

3. **MySqlProvider** (future)
   - Uses Pomelo.EntityFrameworkCore.MySql
   - JSON type support in MySQL 8.0+

### Configuration

```csharp
// Startup.cs / Program.cs
services.AddDbContext<FormBuilderContext>((sp, options) => {
    var provider = sp.GetRequiredService<IDataProvider>();
    provider.ConfigureMigrations(options);
});

// PostgreSQL example
services.AddSingleton<IDataProvider, PostgreSqlProvider>();
```

---

## Data Validation Rules

### FormTemplate
- Name: 1-200 characters, required
- Description: 0-1000 characters, optional
- Version: >= 1
- Soft deletes: IsDeleted flag, no hard deletes if submissions exist

### FormControl
- Label: 1-200 characters, required
- ValidationRules JSON must validate against JSON Schema
- Position must have valid x, y coordinates
- Order must be unique within template (enforced at application layer)

### FormInstance
- TemplateVersion must match an existing template version
- Status transitions: Draft → Submitted only
- SubmittedDate required if Status = Submitted

### FormInstance (Submission Data)
- SubmissionData must be valid JSON object
- Each key must correspond to a valid ControlId in the template
- Each value must include "value" and "type" fields
- Type must match control type expectations
- Values must validate against control's ValidationRules
- Required fields (IsRequired=true) must be present in submitted data

---

## Multi-Tenancy

All entities except ControlLibrary include `TenantId` for isolation:
- Row-level security via query filters
- Indexes include TenantId for query performance
- Cross-tenant queries blocked at EF Core level

```csharp
// Global query filter
modelBuilder.Entity<FormTemplate>()
    .HasQueryFilter(t => t.TenantId == _currentTenantId);
```

---

## Performance Considerations

### Indexing Strategy
- All foreign keys indexed
- Composite indexes for common queries:
  - (TenantId, IsDeleted, ModifiedDate) for template lists
  - (InstanceId, ControlId) unique for submissions
  - (TemplateId, Version) for version lookups

### JSON Column Optimization
- PostgreSQL: Use JSONB for indexed queries on properties
- Limit JSON nesting depth to 3 levels
- Extract frequently queried JSON fields to separate columns if needed

### Caching
- ControlLibrary: Cached in memory (rarely changes)
- Template list queries: Cached with invalidation on template changes
- Submission data: No caching (always fresh from DB)

### Archival Strategy
- Soft delete templates (IsDeleted flag)
- Archive submitted instances older than retention period to cold storage
- Maintain version chain integrity (don't delete versioned templates)

---

## Migration Strategy

### Initial Migration
1. Create all tables with indexes
2. Seed ControlLibrary with default control types
3. Create test tenant with sample templates

### Versioning Migrations
- Never drop columns (add IsDeleted flags)
- Additive changes only (new columns nullable)
- Maintain backward compatibility for NuGet consumers

### Data Migration Scripts
- Template format migrations via SQL scripts
- JSON schema migrations with fallback defaults
- Version number recalculation if needed

---

## Next Steps

1. Implement Entity Framework Core models in `FormR.Core/Models/`
2. Create `FormBuilderContext : DbContext` with entity configurations
3. Implement provider pattern in `FormR.Data/Providers/`
4. Generate initial EF Core migration for PostgreSQL
5. Define API contracts in `contracts/` directory
6. Create quickstart.md with model usage examples
