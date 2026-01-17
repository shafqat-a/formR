using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormR.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedControlLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Basic Inputs
            migrationBuilder.InsertData(
                table: "ControlLibrary",
                columns: new[] { "Type", "Category", "Icon", "ConfigSchema", "DefaultProps" },
                values: new object[,]
                {
                    { "TextInput", 0, "text-input", "{\"properties\":{\"maxLength\":{\"type\":\"number\"},\"minLength\":{\"type\":\"number\"},\"pattern\":{\"type\":\"string\"}}}", "{\"width\":200,\"height\":40}" },
                    { "MultilineText", 0, "textarea", "{\"properties\":{\"rows\":{\"type\":\"number\"},\"maxLength\":{\"type\":\"number\"}}}", "{\"width\":200,\"height\":100,\"rows\":4}" },
                    { "NumberInput", 0, "number", "{\"properties\":{\"min\":{\"type\":\"number\"},\"max\":{\"type\":\"number\"},\"step\":{\"type\":\"number\"}}}", "{\"width\":150,\"height\":40}" },
                    { "EmailInput", 0, "email", "{\"properties\":{\"pattern\":{\"type\":\"string\"}}}", "{\"width\":200,\"height\":40}" },
                    { "PhoneInput", 0, "phone", "{\"properties\":{\"pattern\":{\"type\":\"string\"},\"mask\":{\"type\":\"string\"}}}", "{\"width\":150,\"height\":40}" },

                    // Selection Controls
                    { "Dropdown", 1, "dropdown", "{\"properties\":{\"options\":{\"type\":\"array\"},\"multiple\":{\"type\":\"boolean\"}}}", "{\"width\":200,\"height\":40,\"options\":[]}" },
                    { "Checkbox", 1, "checkbox", "{\"properties\":{\"checked\":{\"type\":\"boolean\"}}}", "{\"width\":150,\"height\":30}" },
                    { "RadioGroup", 1, "radio", "{\"properties\":{\"options\":{\"type\":\"array\"},\"inline\":{\"type\":\"boolean\"}}}", "{\"width\":200,\"height\":80,\"options\":[]}" },
                    { "MultiSelect", 1, "multi-select", "{\"properties\":{\"options\":{\"type\":\"array\"},\"maxSelections\":{\"type\":\"number\"}}}", "{\"width\":200,\"height\":150,\"options\":[]}" },

                    // Date/Time Controls
                    { "DatePicker", 2, "calendar", "{\"properties\":{\"minDate\":{\"type\":\"string\"},\"maxDate\":{\"type\":\"string\"},\"format\":{\"type\":\"string\"}}}", "{\"width\":200,\"height\":40,\"format\":\"yyyy-MM-dd\"}" },
                    { "TimePicker", 2, "clock", "{\"properties\":{\"format\":{\"type\":\"string\"},\"use24Hour\":{\"type\":\"boolean\"}}}", "{\"width\":150,\"height\":40,\"format\":\"HH:mm\"}" },
                    { "DateRangePicker", 2, "date-range", "{\"properties\":{\"minDate\":{\"type\":\"string\"},\"maxDate\":{\"type\":\"string\"}}}", "{\"width\":300,\"height\":40}" },

                    // Advanced Controls
                    { "FileUpload", 3, "upload", "{\"properties\":{\"accept\":{\"type\":\"string\"},\"maxSize\":{\"type\":\"number\"},\"multiple\":{\"type\":\"boolean\"}}}", "{\"width\":200,\"height\":80}" },
                    { "RichTextEditor", 3, "rich-text", "{\"properties\":{\"toolbar\":{\"type\":\"array\"},\"maxLength\":{\"type\":\"number\"}}}", "{\"width\":400,\"height\":200}" },
                    { "RatingScale", 3, "star", "{\"properties\":{\"max\":{\"type\":\"number\"},\"icon\":{\"type\":\"string\"}}}", "{\"width\":150,\"height\":40,\"max\":5}" },
                    { "Slider", 3, "slider", "{\"properties\":{\"min\":{\"type\":\"number\"},\"max\":{\"type\":\"number\"},\"step\":{\"type\":\"number\"}}}", "{\"width\":200,\"height\":40,\"min\":0,\"max\":100,\"step\":1}" },
                    { "SignaturePad", 3, "signature", "{\"properties\":{\"penColor\":{\"type\":\"string\"},\"backgroundColor\":{\"type\":\"string\"}}}", "{\"width\":300,\"height\":150}" },

                    // Layout Controls
                    { "Section", 4, "section", "{\"properties\":{\"title\":{\"type\":\"string\"},\"collapsible\":{\"type\":\"boolean\"}}}", "{\"width\":400,\"height\":200}" },
                    { "ColumnContainer", 4, "columns", "{\"properties\":{\"columns\":{\"type\":\"number\"},\"gap\":{\"type\":\"number\"}}}", "{\"width\":400,\"height\":200,\"columns\":2}" },
                    { "TabPanel", 4, "tabs", "{\"properties\":{\"tabs\":{\"type\":\"array\"}}}", "{\"width\":400,\"height\":300}" },
                    { "Accordion", 4, "accordion", "{\"properties\":{\"items\":{\"type\":\"array\"},\"allowMultiple\":{\"type\":\"boolean\"}}}", "{\"width\":400,\"height\":200}" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ControlLibrary",
                keyColumn: "Type",
                keyValues: new object[]
                {
                    "TextInput", "MultilineText", "NumberInput", "EmailInput", "PhoneInput",
                    "Dropdown", "Checkbox", "RadioGroup", "MultiSelect",
                    "DatePicker", "TimePicker", "DateRangePicker",
                    "FileUpload", "RichTextEditor", "RatingScale", "Slider", "SignaturePad",
                    "Section", "ColumnContainer", "TabPanel", "Accordion"
                });
        }
    }
}
