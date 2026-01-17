using Microsoft.EntityFrameworkCore;

namespace FormR.Data.Providers;

public interface IDataProvider
{
    DbContext CreateContext();
    void ConfigureMigrations(DbContextOptionsBuilder options);
    string GetProviderName();
}
