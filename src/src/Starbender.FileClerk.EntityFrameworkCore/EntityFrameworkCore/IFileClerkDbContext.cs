using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Starbender.FileClerk.EntityFrameworkCore;

[ConnectionStringName(FileClerkDbProperties.ConnectionStringName)]
public interface IFileClerkDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
