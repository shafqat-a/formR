using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormR.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ControlLibrary",
                columns: table => new
                {
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ConfigSchema = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    DefaultProps = table.Column<JsonDocument>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlLibrary", x => x.Type);
                });

            migrationBuilder.CreateTable(
                name: "FormTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    BaseTemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormTemplates_FormTemplates_BaseTemplateId",
                        column: x => x.BaseTemplateId,
                        principalTable: "FormTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormControls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Placeholder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DefaultValue = table.Column<string>(type: "text", nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    ValidationRules = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    Position = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    Properties = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    ParentControlId = table.Column<Guid>(type: "uuid", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormControls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormControls_FormControls_ParentControlId",
                        column: x => x.ParentControlId,
                        principalTable: "FormControls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormControls_FormTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "FormTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormControls_ParentControlId",
                table: "FormControls",
                column: "ParentControlId");

            migrationBuilder.CreateIndex(
                name: "IX_FormControls_TemplateId",
                table: "FormControls",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_FormTemplates_BaseTemplateId",
                table: "FormTemplates",
                column: "BaseTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_FormTemplates_IsDeleted",
                table: "FormTemplates",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FormTemplates_Name_Version_TenantId",
                table: "FormTemplates",
                columns: new[] { "Name", "Version", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormTemplates_TenantId",
                table: "FormTemplates",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControlLibrary");

            migrationBuilder.DropTable(
                name: "FormControls");

            migrationBuilder.DropTable(
                name: "FormTemplates");
        }
    }
}
