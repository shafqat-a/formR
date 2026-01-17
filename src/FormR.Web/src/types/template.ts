export interface TemplateListDto {
  id: string;
  name: string;
  description?: string;
  version: number;
  createdDate: string;
  modifiedDate: string;
  controlCount: number;
}

export interface TemplateDetailDto {
  id: string;
  name: string;
  description?: string;
  version: number;
  baseTemplateId?: string;
  createdDate: string;
  modifiedDate: string;
  controls: ControlDto[];
}

export interface CreateTemplateDto {
  name: string;
  description?: string;
  controls: CreateControlDto[];
}

export interface UpdateTemplateDto {
  name: string;
  description?: string;
  version: number;
  controls: UpdateControlDto[];
}

export interface ControlDto {
  id: string;
  type: string;
  label: string;
  placeholder?: string;
  defaultValue?: string;
  isRequired: boolean;
  validationRules?: Record<string, any>;
  position: Position;
  properties?: Record<string, any>;
  parentControlId?: string;
  order: number;
}

export interface CreateControlDto {
  type: string;
  label: string;
  placeholder?: string;
  defaultValue?: string;
  isRequired: boolean;
  validationRules?: Record<string, any>;
  position: Position;
  properties?: Record<string, any>;
  parentControlId?: string;
  order: number;
}

export interface UpdateControlDto {
  id: string;
  type: string;
  label: string;
  placeholder?: string;
  defaultValue?: string;
  isRequired: boolean;
  validationRules?: Record<string, any>;
  position: Position;
  properties?: Record<string, any>;
  parentControlId?: string;
  order: number;
}

export interface Position {
  x: number;
  y: number;
  width: number;
  height: number;
  zIndex?: number;
}

export interface ControlLibraryDto {
  type: string;
  category: string;
  icon: string;
  configSchema: Record<string, any>;
  defaultProps: Record<string, any>;
}

export type ControlType =
  | 'TextInput'
  | 'MultilineText'
  | 'NumberInput'
  | 'EmailInput'
  | 'PhoneInput'
  | 'Dropdown'
  | 'Checkbox'
  | 'RadioGroup'
  | 'MultiSelect'
  | 'DatePicker'
  | 'TimePicker'
  | 'DateRangePicker'
  | 'FileUpload'
  | 'RichTextEditor'
  | 'RatingScale'
  | 'Slider'
  | 'SignaturePad'
  | 'Section'
  | 'ColumnContainer'
  | 'TabPanel'
  | 'Accordion';

export type ControlCategory =
  | 'BasicInputs'
  | 'SelectionControls'
  | 'DateTimeControls'
  | 'AdvancedControls'
  | 'LayoutControls';
