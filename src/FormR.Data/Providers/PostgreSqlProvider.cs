using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FormR.Data.Providers;

public class PostgreSqlProvider : IDataProvider
{
    private readonly IConfiguration _configuration;

    public PostgreSqlProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbContext CreateContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FormBuilderContext>();
        ConfigureMigrations(optionsBuilder);
        return new FormBuilderContext(optionsBuilder.Options);
    }

    public void ConfigureMigrations(DbContextOptionsBuilder options)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly("FormR.Data");
            npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }

    public string GetProviderName()
    {
        return "PostgreSQL";
    }
}
