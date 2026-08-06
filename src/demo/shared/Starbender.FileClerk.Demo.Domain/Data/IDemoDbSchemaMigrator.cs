using System.Threading.Tasks;

namespace Starbender.FileClerk.Demo.Data;

public interface IDemoDbSchemaMigrator
{
    Task MigrateAsync();
}
