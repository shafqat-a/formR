using FormR.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FormR.Data;

public class FormBuilderContext : DbContext
{
    public FormBuilderContext(DbContextOptions<FormBuilderContext> options)
        : base(options)
    {
    }

    public DbSet<FormTemplate> FormTemplates { get; set; }
    public DbSet<FormControl> FormControls { get; set; }
    public DbSet<ControlLibrary> ControlLibrary { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // FormTemplate configuration
        modelBuilder.Entity<FormTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.Name, e.Version, e.TenantId }).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.BaseTemplateId);
            entity.HasIndex(e => e.IsDeleted);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.ModifiedDate).IsRequired();

            // Self-referencing relationship for versioning
            entity.HasOne(e => e.BaseTemplate)
                .WithMany(e => e.DerivedTemplates)
                .HasForeignKey(e => e.BaseTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many with FormControls (cascade delete)
            entity.HasMany(e => e.Controls)
                .WithOne(e => e.Template)
                .HasForeignKey(e => e.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FormControl configuration
        modelBuilder.Entity<FormControl>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.TemplateId);
            entity.HasIndex(e => e.ParentControlId);

            entity.Property(e => e.TemplateId).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Label).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Placeholder).HasMaxLength(200);

            // JSON columns for PostgreSQL
            entity.Property(e => e.Position)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(e => e.ValidationRules)
                .HasColumnType("jsonb");

            entity.Property(e => e.Properties)
                .HasColumnType("jsonb");

            // Self-referencing relationship for nested controls
            entity.HasOne(e => e.ParentControl)
                .WithMany(e => e.ChildControls)
                .HasForeignKey(e => e.ParentControlId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ControlLibrary configuration (reference data)
        modelBuilder.Entity<ControlLibrary>(entity =>
        {
            entity.HasKey(e => e.Type);

            entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Icon).HasMaxLength(50).IsRequired();

            // JSON columns for PostgreSQL
            entity.Property(e => e.ConfigSchema)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(e => e.DefaultProps)
                .HasColumnType("jsonb")
                .IsRequired();
        });
    }
}
