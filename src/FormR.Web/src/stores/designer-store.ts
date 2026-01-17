import { create } from 'zustand';
import type {
  ControlDto,
  ControlLibraryDto,
  TemplateDetailDto,
  Position,
} from '../types/template';

export interface DesignerControl extends ControlDto {
  // Additional UI state for designer
  isSelected?: boolean;
  isDragging?: boolean;
}

interface DesignerState {
  // Template being edited
  template: TemplateDetailDto | null;
  templateName: string;
  templateDescription: string;

  // Controls on the canvas
  controls: DesignerControl[];
  selectedControlId: string | null;

  // Control library
  controlLibrary: ControlLibraryDto[];

  // UI state
  isLoading: boolean;
  error: string | null;
  isDirty: boolean; // Has unsaved changes

  // Actions
  setTemplate: (template: TemplateDetailDto) => void;
  setTemplateName: (name: string) => void;
  setTemplateDescription: (description: string) => void;
  setControlLibrary: (library: ControlLibraryDto[]) => void;

  addControl: (control: DesignerControl) => void;
  updateControl: (id: string, updates: Partial<DesignerControl>) => void;
  removeControl: (id: string) => void;
  selectControl: (id: string | null) => void;
  clearSelection: () => void;

  updateControlPosition: (id: string, position: Position) => void;
  updateControlProperty: (id: string, key: string, value: any) => void;

  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
  resetTemplate: () => void;
  markDirty: () => void;
  markClean: () => void;
}

export const useDesignerStore = create<DesignerState>((set) => ({
  // Initial state
  template: null,
  templateName: '',
  templateDescription: '',
  controls: [],
  selectedControlId: null,
  controlLibrary: [],
  isLoading: false,
  error: null,
  isDirty: false,

  // Template actions
  setTemplate: (template) =>
    set({
      template,
      templateName: template.name,
      templateDescription: template.description || '',
      controls: template.controls.map((c) => ({ ...c, isSelected: false })),
      selectedControlId: null,
      isDirty: false,
    }),

  setTemplateName: (name) =>
    set((state) => ({
      templateName: name,
      isDirty: state.template ? name !== state.template.name : true,
    })),

  setTemplateDescription: (description) =>
    set((state) => ({
      templateDescription: description,
      isDirty: state.template ? description !== (state.template.description || '') : true,
    })),

  setControlLibrary: (library) =>
    set({ controlLibrary: library }),

  // Control actions
  addControl: (control) =>
    set((state) => ({
      controls: [...state.controls, control],
      isDirty: true,
    })),

  updateControl: (id, updates) =>
    set((state) => ({
      controls: state.controls.map((c) =>
        c.id === id ? { ...c, ...updates } : c
      ),
      isDirty: true,
    })),

  removeControl: (id) =>
    set((state) => ({
      controls: state.controls.filter((c) => c.id !== id),
      selectedControlId: state.selectedControlId === id ? null : state.selectedControlId,
      isDirty: true,
    })),

  selectControl: (id) =>
    set((state) => ({
      selectedControlId: id,
      controls: state.controls.map((c) => ({
        ...c,
        isSelected: c.id === id,
      })),
    })),

  clearSelection: () =>
    set((state) => ({
      selectedControlId: null,
      controls: state.controls.map((c) => ({
        ...c,
        isSelected: false,
      })),
    })),

  updateControlPosition: (id, position) =>
    set((state) => ({
      controls: state.controls.map((c) =>
        c.id === id ? { ...c, position } : c
      ),
      isDirty: true,
    })),

  updateControlProperty: (id, key, value) =>
    set((state) => ({
      controls: state.controls.map((c) => {
        if (c.id === id) {
          // Handle nested property updates
          if (key.includes('.')) {
            const [parent, child] = key.split('.');
            return {
              ...c,
              [parent]: {
                ...(c[parent as keyof DesignerControl] as any),
                [child]: value,
              },
            };
          }
          return { ...c, [key]: value };
        }
        return c;
      }),
      isDirty: true,
    })),

  // UI state actions
  setLoading: (loading) => set({ isLoading: loading }),
  setError: (error) => set({ error }),

  resetTemplate: () =>
    set({
      template: null,
      templateName: '',
      templateDescription: '',
      controls: [],
      selectedControlId: null,
      isDirty: false,
      error: null,
    }),

  markDirty: () => set({ isDirty: true }),
  markClean: () => set({ isDirty: false }),
}));

export default useDesignerStore;
