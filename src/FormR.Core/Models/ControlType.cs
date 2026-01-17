namespace FormR.Core.Models;

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
