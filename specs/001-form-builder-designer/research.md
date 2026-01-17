# Research: Form Builder with Visual Designer

**Date**: 2026-01-18
**Phase**: 0 - Research & Technology Selection
**Related**: [plan.md](./plan.md), [spec.md](./spec.md)

## Overview

This document consolidates research on technology choices and best practices for the visual form builder. Research was conducted to resolve NEEDS CLARIFICATION items from Technical Context and to analyze competitive form/page builders for feature parity.

## 1. Frontend Framework Selection

### Decision: React

**Chosen**: React 19 with TypeScript

**Rationale**:
1. **Drag-and-Drop Ecosystem**: React has the most mature drag-and-drop libraries (dnd-kit, react-grid-layout) specifically designed for visual canvas interactions
2. **Performance**: Proven performance for complex UIs with 50+ controls through virtual DOM, concurrent mode, and automatic batching
3. **Simplicity**: Component-based architecture with hooks provides excellent developer experience while meeting "should be simple" requirement
4. **Talent Pool**: Largest hiring pool and community support ensures long-term maintainability
5. **TypeScript Support**: First-class TypeScript integration with optional adoption path
6. **Bundle Size**: 44.5kb gzipped (acceptable tradeoff vs Vue's 34.7kb given ecosystem benefits)

**Alternatives Considered**:

| Framework | Why Rejected |
|-----------|-------------|
| Vue | Smaller drag-and-drop ecosystem, fewer canvas-oriented libraries. While offering better bundle size (34.7kb) and DX, lacks mature form builder libraries equivalent to dnd-kit. Smaller talent pool. |
| Angular | Largest bundle size (62.3kb), steepest learning curve, opinionated structure conflicts with NuGet library flexibility requirements. Best for teams already standardized on Angular. |
| Vanilla JavaScript | Requires building drag-and-drop, state management, and reactivity from scratch. Violates "simple" requirement and significantly increases development time. No viable path to 50+ control performance. |
| Svelte | Excellent performance and smallest bundle, but immature ecosystem for enterprise drag-and-drop builders. Difficult hiring. |

**Sources**:
- [React vs Vue vs Angular 2026 Performance Guide](https://blog.logrocket.com/angular-vs-react-vs-vue-js-performance/)
- [React vs Vue vs Angular - Practical Guide 2026](https://brillcreations.com/react-vs-vue-vs-angular-in-2026-a-practical-guide-for-developers/)

---

## 2. Frontend Testing Framework Selection

### Decision: Vitest + Playwright

**Unit/Component Testing**: Vitest with React Testing Library

**Rationale**:
1. **Performance**: 10-20x faster than Jest in watch mode, 30-70% faster test runtime
2. **Modern Architecture**: Built for Vite with native ESM support and HMR for tests
3. **Compatibility**: 95% compatible with Jest, seamless integration with React Testing Library
4. **Future-Proof**: Leading the future of JavaScript testing as ecosystem moves to ESM
5. **Lightweight**: Minimal overhead and bundle size

**E2E Testing**: Playwright

**Rationale**:
1. **Cross-Browser Support**: Chrome, Firefox, Safari (WebKit) - critical for browser compatibility requirement
2. **Performance**: Native parallelization (test suite improvements: 90min → 14min)
3. **Multi-Language Support**: Python, C#, Java support aligns with .NET 10 backend team skills
4. **Advanced Features**: Multiple sessions/contexts, mobile testing, network interception
5. **Enterprise-Ready**: Better for large-scale, complex testing environments

**Alternatives Considered**:

| Tool | Why Rejected |
|------|-------------|
| Jest (unit) | Slower performance, experimental ESM support, legacy architecture. Vitest's 95% compatibility mitigates ecosystem maturity concerns. |
| Cypress (E2E) | Limited to Chrome, Firefox, Edge (no Safari/mobile), no native parallelization, JavaScript-only. Better for rapid development/small teams, but insufficient for 100 concurrent users requirement. |
| Selenium (E2E) | Legacy architecture, slower, verbose API. Playwright is the modern replacement. |

**Testing Strategy**:
```
├── Unit Tests (Vitest)
│   ├── Individual React components
│   ├── Utility functions, validation logic
│   └── Fast feedback (<100ms per test)
├── Component Tests (Vitest + Testing Library)
│   ├── Drag-and-drop interactions
│   ├── Property panel updates
│   └── Form control rendering
└── E2E Tests (Playwright)
    ├── Full user workflows (designer → instance → submit)
    ├── Cross-browser validation
    ├── File uploads, complex interactions
    └── Performance benchmarks (SC-002: <2s template load)
```

**Sources**:
- [Jest vs Vitest 2025 Comparison](https://medium.com/@ruverd/jest-vs-vitest-which-test-runner-should-you-use-in-2025-5c85e4f2bda9)
- [Vitest vs Jest - Better Stack](https://betterstack.com/community/guides/scaling-nodejs/vitest-vs-jest/)
- [Playwright vs Cypress 2026 Enterprise Guide](https://devin-rosario.medium.com/playwright-vs-cypress-the-2026-enterprise-testing-guide-ade8b56d3478)

---

## 3. Drag-and-Drop Libraries

### Decision: dnd-kit (primary) + react-grid-layout (secondary)

**Primary**: dnd-kit

**Rationale**:
1. **Performance**: 10kb minified, zero external dependencies, optimized for complex UIs
2. **Flexibility**: Highly customizable collision detection, sensors, animations
3. **Canvas Support**: Built for visual designers (used in Puck editor)
4. **TypeScript**: First-class TypeScript support
5. **Required Features**:
   - Palette → Canvas dragging
   - Repositioning elements
   - Nesting/containment with nested contexts
   - Snap-to-grid modifiers (4px, 8px, 12px, 16px grids)
   - Multiple containers (palette, canvas sections)
   - Built-in accessibility

**Secondary**: react-grid-layout

**Use Case**: Grid-based layout controls (multi-column sections, responsive breakpoints)

**Rationale**:
1. **Built-in Resize**: Handles drag AND resize (1.5M weekly downloads)
2. **Responsive Grids**: Breakpoint-based layouts (desktop/tablet/mobile)
3. **Mature**: Proven in dashboard builders

**Library Usage Matrix**:

| Scenario | Library |
|----------|---------|
| Palette → Canvas dragging | dnd-kit |
| Freeform canvas positioning | dnd-kit |
| Grid-based multi-column layouts | react-grid-layout |
| Resizable sections | react-grid-layout |
| Nested containers | dnd-kit |
| Snap-to-grid alignment | dnd-kit (with modifiers) |

**Alternatives Considered**:

| Library | Why Rejected |
|---------|-------------|
| react-beautiful-dnd | Deprecated by Atlassian in favor of Pragmatic DnD. No longer maintained. |
| React DnD | Older architecture, less performant than dnd-kit, more complex API. 1.3M weekly downloads but declining. |
| Pragmatic DnD | Framework-agnostic (pro/con), smallest bundle, but less React-specific optimization. Better for multi-framework products. |
| HTML5 Drag-and-Drop API | Poor touch support, no snap-to-grid, requires significant custom code for canvas interactions. |

**Implementation Example**:
```javascript
import { createSnapModifier } from '@dnd-kit/modifiers';

const gridSize = 8; // 8px grid (Figma/Webflow standard)
const snapToGrid = createSnapModifier(gridSize);
```

**Sources**:
- [Top 5 React Drag-and-Drop Libraries 2026](https://puckeditor.com/blog/top-5-drag-and-drop-libraries-for-react)
- [dnd-kit Documentation](https://docs.dndkit.com)
- [react-grid-layout npm trends](https://npmtrends.com/@dnd-kit/core-vs-react-dnd-vs-react-grid-layout)

---

## 4. Visual Designer Feature Analysis

### Competitive Analysis Summary

Analyzed leading visual builders: Wix, Squarespace, Webflow, Typeform, Google Forms, JotForm

### Must-Have Visual Designer Features

#### Core Canvas Features

1. **Drag-and-Drop Interface**
   - Drag controls from palette to canvas
   - Reposition elements on canvas
   - Visual drop zones with feedback
   - Snap-to-grid alignment (4px, 8px, 12px, 16px spacing)

2. **Grid & Layout System**
   - 12-column responsive grid (industry standard)
   - Flexbox-style layout controls
   - Auto-spacing with gap controls (row/column gaps)
   - Alignment tools (left, center, right, top, middle, bottom)

3. **Visual Feedback & Guides**
   - Alignment guides (Figma-style snap indicators)
   - Bounding boxes on selection
   - Distance measurements between elements
   - Hover states showing interactive areas
   - Grid overlay (toggleable)

4. **Element Manipulation**
   - Multi-select (Shift+click, drag-to-select box)
   - Resize handles for applicable controls
   - Z-index controls (bring to front/back)
   - Copy/paste/duplicate controls
   - Undo/redo support

#### Control Library Organization

5. **Categorized Control Palette**
   - **Basic Inputs**: Text input, multi-line text, number, email, phone
   - **Selection Controls**: Dropdown, checkbox, radio buttons, multi-select
   - **Date/Time**: Date picker, time picker, date range
   - **Advanced**: File upload, rich text editor, rating scale, slider
   - **Layout Controls**: Section/container, columns, tabs, accordion
   - **Special**: Conditional fields, calculated fields, signature pad

6. **Properties Panel**
   - Context-sensitive based on selected control
   - Visual property editors (color pickers, font selectors)
   - Live preview of changes
   - Validation rule configuration
   - Conditional logic builder
   - Default values and placeholders

#### Advanced Features

7. **Container & Nesting**
   - Nested containers for complex layouts
   - Section headers/dividers
   - Multi-column layouts
   - Tabs and accordion panels

8. **Responsive Design Controls**
   - Desktop/tablet/mobile preview modes
   - Breakpoint management
   - Device-specific visibility toggles

9. **Template Features**
   - Template preview/thumbnail generation
   - Template duplication
   - Template categorization/folders
   - Search/filter templates

10. **Collaboration & Quality**
    - Auto-save (draft state)
    - Change history/version control
    - Validation warnings (missing required properties)
    - Accessibility checks

### Competitive Feature Matrix

| Feature Category | Webflow | Wix | Squarespace | JotForm | Typeform | FormR (Target) |
|-----------------|---------|-----|-------------|---------|----------|----------------|
| Visual CSS Control | ✓✓✓ | ✓ | ✓ | ✗ | ✗ | ✓ (via property panel) |
| Grid-based Layout | ✓✓✓ | ✓ | ✓✓✓ | ✓ | ✗ | ✓✓✓ (12-column + flexbox) |
| Snap Alignment | ✓✓ | ✓✓✓ | ✓✓ | ✓ | ✗ | ✓✓✓ (8px grid standard) |
| Conditional Logic | ✓ | ✓ | ✗ | ✓✓✓ | ✓✓✓ | ✓✓ (form-specific) |
| Template Library | ✓✓ | ✓✓✓ | ✓✓ | ✓✓✓ | ✓ | ✓✓ (custom templates) |
| Nesting/Containers | ✓✓✓ | ✓✓ | ✓ | ✓✓ | ✗ | ✓✓ (sections, columns) |

**Legend**: ✓ = Basic, ✓✓ = Good, ✓✓✓ = Excellent, ✗ = None/Poor

**Sources**:
- [Webflow vs Wix 2026 Comparison](https://www.slammedialab.com/post/webflow-vs-wix)
- [Squarespace vs Wix 2026](https://www.abzglobal.net/squarespace-blog/squarespace-vs-wix-in-2026-which-website-builder-should-you-choose)
- [JotForm vs Google Forms 2026](https://www.involve.me/blog/jotform-vs-google-forms)
- [Typeform vs JotForm 2026](https://www.involve.me/blog/jotform-vs-typeform-vs-involveme)

---

## 5. Additional Technology Decisions

### State Management

**Recommendation**: Zustand or Redux Toolkit (TBD during implementation)

**Considerations**:
- **Zustand**: Lightweight (1kb), simple API, sufficient for form builder state
- **Redux Toolkit**: More established, better DevTools, team familiarity
- **React Context**: Sufficient for small apps but may have performance issues with frequent canvas updates

### UI Component Library

**Recommendation**: Material-UI (MUI) or Chakra UI (TBD during implementation)

**Considerations**:
- **Material-UI**: Comprehensive components, good accessibility, larger bundle
- **Chakra UI**: Smaller bundle, excellent TypeScript support, modern design
- **Custom Components**: Maximum flexibility but higher development cost

### Build Tool

**Decision**: Vite

**Rationale**:
- Native ESM support
- Fast HMR for development
- Optimized production builds
- Aligns with Vitest testing framework
- Industry standard for modern React apps

---

## Technology Stack Summary

```
┌──────────────────────────────────────────────────────────┐
│ FormR Technology Stack                                   │
├──────────────────────────────────────────────────────────┤
│ Frontend:     React 19 + TypeScript                      │
│ Build Tool:   Vite                                       │
│ UI Framework: Material-UI or Chakra UI (TBD)             │
│ Drag & Drop:  dnd-kit + react-grid-layout                │
│ State:        Zustand or Redux Toolkit (TBD)             │
│ Testing:      Vitest + Playwright + Testing Library      │
├──────────────────────────────────────────────────────────┤
│ Backend:      .NET 10 + ASP.NET Core                     │
│ Data Access:  Entity Framework Core (provider pattern)   │
│ Database:     PostgreSQL (default), SQL Server (future)  │
│ Testing:      xUnit + FluentAssertions + Testcontainers  │
├──────────────────────────────────────────────────────────┤
│ Packaging:    NuGet (FormR.Core, FormR.Data)             │
│ Distribution: Docker + Kubernetes (optional)             │
└──────────────────────────────────────────────────────────┘
```

---

## Resolved Clarifications

### From Technical Context (plan.md)

1. **Frontend Framework**: React 19 with TypeScript (resolved)
2. **Frontend Testing Framework**: Vitest (unit/component) + Playwright (E2E) (resolved)

Both NEEDS CLARIFICATION items have been resolved through comprehensive research and competitive analysis.

---

## Next Steps

1. **Phase 1: Design**
   - Create data model (Entity Framework Core entities)
   - Define API contracts (OpenAPI/Swagger)
   - Design React component architecture
   - Set up project structure

2. **Phase 2: Tasks**
   - Generate actionable tasks with `/speckit.tasks`
   - Organize by user story priority (P1→P2→P3→P4)

3. **Phase 3: Implementation**
   - Execute tasks with test-first approach per constitution
   - Write failing tests before implementation
   - Follow GitHub workflow (issue #1)
