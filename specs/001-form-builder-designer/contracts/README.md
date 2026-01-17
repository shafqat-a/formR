# API Contracts

**Date**: 2026-01-18
**Phase**: 1 - Design
**Related**: [api-spec.yaml](./api-spec.yaml), [data-model.md](../data-model.md)

## Overview

This directory contains the API contract specifications for the FormR REST API. The API follows RESTful principles and is documented using OpenAPI 3.0.

## Files

- **api-spec.yaml**: OpenAPI 3.0 specification for the complete REST API
- **README.md**: This file - API design documentation

## API Design Principles

### 1. RESTful Resource Organization

The API is organized around three primary resources:

- **Templates** (`/templates`): Form template management
- **Forms** (`/forms`): Form instance creation and management
- **Submissions** (`/submissions`): Submission data retrieval
- **Controls** (`/controls`): Control library metadata

### 2. Versioning Strategy

- **URL Versioning**: `/v1/templates`, `/v2/templates`
- **Initial Version**: v1.0.0
- **Breaking Changes**: Require new major version
- **Backward Compatibility**: Maintained within major version

### 3. HTTP Methods

| Method | Usage |
|--------|-------|
| GET | Retrieve resource(s) |
| POST | Create new resource or trigger action |
| PUT | Update existing resource (full replacement) |
| PATCH | Partial update (not used in v1) |
| DELETE | Soft delete resource |

### 4. Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| 200 | OK | Successful GET, PUT, or action |
| 201 | Created | Successful POST creating resource |
| 204 | No Content | Successful DELETE |
| 400 | Bad Request | Invalid request syntax or parameters |
| 401 | Unauthorized | Missing or invalid authentication |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Resource state conflict (e.g., already submitted) |
| 422 | Unprocessable Entity | Validation errors |
| 500 | Internal Server Error | Server failure |

### 5. Response Format

All responses follow consistent JSON structure:

**Success Response**:
```json
{
  "id": "guid",
  "...": "resource fields"
}
```

**List Response**:
```json
{
  "items": [...],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 100,
    "totalPages": 5
  }
}
```

**Error Response**:
```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Human-readable message",
    "details": {}
  }
}
```

### 6. Pagination

List endpoints support pagination:

- **Query Parameters**: `page` (1-indexed), `pageSize` (1-100, default 20)
- **Response**: Includes `pagination` object with metadata
- **Performance**: Indexed queries for efficient pagination

### 7. Filtering & Search

- **Filtering**: Query parameters (e.g., `?folderId=guid`, `?status=Draft`)
- **Search**: `?search=query` for full-text search on names/descriptions
- **Date Ranges**: `?submittedAfter=ISO8601&submittedBefore=ISO8601`

### 8. Security

- **Authentication**: Bearer JWT tokens (`Authorization: Bearer <token>`)
- **Multi-Tenancy**: Automatic tenant isolation via JWT claims
- **CORS**: Configurable allowed origins
- **Rate Limiting**: Per-tenant limits (TBD in implementation)

## Key API Flows

### Flow 1: Create Template

```
POST /v1/templates
{
  "name": "Customer Feedback",
  "controls": [
    {
      "type": "TextInput",
      "label": "Name",
      "position": {"x": 0, "y": 0},
      "order": 1,
      "isRequired": true
    }
  ]
}

Response 201 Created
Location: /v1/templates/{templateId}
{
  "id": "{templateId}",
  "name": "Customer Feedback",
  "version": 1,
  "controls": [...],
  "createdDate": "2026-01-18T10:00:00Z"
}
```

### Flow 2: Instantiate Template & Submit Data

```
1. Create Instance
POST /v1/forms
{
  "templateId": "{templateId}"
}

Response 201 Created
{
  "id": "{instanceId}",
  "templateId": "{templateId}",
  "templateVersion": 1,
  "status": "Draft"
}

2. Submit Data
POST /v1/forms/{instanceId}/submit
{
  "data": {
    "{controlId}": {
      "value": "John Doe",
      "type": "string"
    },
    "{controlId2}": {
      "value": 42,
      "type": "number"
    }
  },
  "submittedBy": "user@example.com"
}

Response 200 OK
{
  "instanceId": "{instanceId}",
  "status": "Submitted",
  "submittedDate": "2026-01-18T10:05:00Z",
  "submissionCount": 2
}
```

### Flow 3: Update Template (with Versioning)

```
PUT /v1/templates/{templateId}
{
  "name": "Customer Feedback (Updated)",
  "controls": [...updated controls...]
}

If template has NO submissions:
Response 200 OK (updated in-place)

If template HAS submissions:
Response 201 Created (new version created)
Location: /v1/templates/{newTemplateId}
{
  "id": "{newTemplateId}",
  "name": "Customer Feedback (Updated)",
  "version": 2,
  "baseTemplateId": "{templateId}",
  ...
}
```

### Flow 4: Retrieve Submissions

```
GET /v1/submissions?templateId={templateId}&page=1&pageSize=20

Response 200 OK
{
  "submissions": [
    {
      "instanceId": "{instanceId}",
      "templateName": "Customer Feedback",
      "submittedDate": "2026-01-18T10:05:00Z",
      "submittedBy": "user@example.com"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 42
  }
}

GET /v1/submissions/{instanceId}

Response 200 OK
{
  "instanceId": "{instanceId}",
  "template": {...full template with controls...},
  "submissionData": {
    "control-1": {
      "value": "John Doe",
      "type": "string"
    },
    "control-2": {
      "value": 42,
      "type": "number"
    }
  },
  "submittedDate": "2026-01-18T10:05:00Z",
  "submittedBy": "user@example.com"
}
```

## Validation Rules

### Template Validation

- **Name**: 1-200 characters, required
- **Description**: 0-1000 characters, optional
- **Controls**: At least 1 control required
- **Control IDs**: Must be unique within template
- **Position**: x, y required, must be non-negative
- **Order**: Must be sequential starting from 1

### Submission Validation

- **Required Fields**: Must have values in submission data
- **Value Types**: Type field must match control type expectations
- **Validation Rules**: Applied per control configuration from template
  - Regex patterns for text fields
  - Min/max length constraints
  - Min/max values for numbers
  - Allowed file types/sizes for uploads
- **JSON Structure**: Submission data must be valid JSON object with controlId keys

### Business Rules

- **Template Deletion**: Blocked if submissions exist (unless `force=true`)
- **Template Versioning**: Automatic when editing template with submissions
- **Instance Submission**: One-time only (Draft → Submitted, no rollback)
- **Submission Immutability**: Cannot edit after status = Submitted

## Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| UNAUTHORIZED | 401 | Missing or invalid authentication |
| FORBIDDEN | 403 | Insufficient permissions |
| NOT_FOUND | 404 | Resource doesn't exist |
| VALIDATION_FAILED | 422 | Input validation errors |
| REQUIRED_FIELD_MISSING | 422 | Required field not provided |
| INVALID_VALUE_TYPE | 422 | Value doesn't match declared type |
| TEMPLATE_HAS_SUBMISSIONS | 400 | Cannot delete template with submissions |
| ALREADY_SUBMITTED | 409 | Form instance already submitted |
| INTERNAL_ERROR | 500 | Unexpected server error |

## Performance Considerations

### Caching Strategy

- **Control Library**: Cached indefinitely (rarely changes)
- **Templates**: Cache with 5-minute TTL, invalidate on update
- **Submissions**: No caching (always fresh data)

### Query Optimization

- **Indexes**: All foreign keys, (TenantId, IsDeleted), (TemplateId, Version)
- **Pagination**: Limit 100 items per page max
- **Eager Loading**: Template includes controls by default
- **Projection**: Use `?fields=id,name` for list views (future optimization)

### Rate Limiting

- **Per Tenant**: 100 requests/minute (configurable)
- **Burst Allowance**: 20 additional requests
- **Response Headers**: `X-RateLimit-Limit`, `X-RateLimit-Remaining`

## API Evolution

### Versioning Roadmap

- **v1.0**: Initial release (current)
- **v1.1**: Add PATCH support for partial updates
- **v1.2**: Add bulk operations (create multiple templates)
- **v2.0**: GraphQL endpoint (optional, for complex queries)

### Backward Compatibility

- **Additive Changes**: New fields, new endpoints (minor version)
- **Breaking Changes**: Remove fields, change semantics (major version)
- **Deprecation Policy**: 6-month notice before removal

## Testing Strategy

### Contract Tests

- **Prism Mock Server**: Use `api-spec.yaml` to generate mock API
- **Schema Validation**: Validate all responses against OpenAPI spec
- **Example Requests**: Test examples in spec for accuracy

### Integration Tests

- **Happy Paths**: All CRUD operations per resource
- **Error Cases**: All documented error codes
- **Validation**: All validation rules enforced
- **Versioning**: Template update creates version correctly

### Load Tests

- **Template List**: 1000 templates, <200ms p95
- **Submission List**: 10,000 submissions, <300ms p95
- **Template Save**: 50 controls, <1s p95
- **Concurrent Users**: 100+ simultaneous requests

## OpenAPI Tooling

### Recommended Tools

- **Swagger UI**: Interactive API documentation (https://swagger.io/tools/swagger-ui/)
- **Stoplight Studio**: Visual OpenAPI editor
- **Prism**: Mock server for frontend development
- **openapi-generator**: Generate client SDKs (C#, TypeScript, Python)

### Usage Examples

```bash
# Validate OpenAPI spec
npx @stoplight/spectral-cli lint api-spec.yaml

# Start mock server
npx @stoplight/prism-cli mock api-spec.yaml

# Generate C# client
openapi-generator-cli generate -i api-spec.yaml -g csharp -o ./generated/csharp-client

# Generate TypeScript client
openapi-generator-cli generate -i api-spec.yaml -g typescript-axios -o ./generated/ts-client
```

## Next Steps

1. Implement ASP.NET Core controllers per spec
2. Add Swashbuckle/NSwag for auto-generated Swagger docs
3. Configure CORS and authentication middleware
4. Implement validation with FluentValidation
5. Add integration tests using WebApplicationFactory
6. Set up API versioning middleware
7. Configure rate limiting with AspNetCoreRateLimit
