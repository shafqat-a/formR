using Microsoft.EntityFrameworkCore;

namespace FormR.Data;

public class FormBuilderContext : DbContext
{
    public FormBuilderContext(DbContextOptions<FormBuilderContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Entity configurations will be added here as each entity is created
    }
}
