using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Starbender.FileClerk.EntityFrameworkCore;

[ConnectionStringName(FileClerkDbProperties.ConnectionStringName)]
public class FileClerkDbContext : AbpDbContext<FileClerkDbContext>, IFileClerkDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public FileClerkDbContext(DbContextOptions<FileClerkDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureFileClerk();
    }
}
