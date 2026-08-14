using Microsoft.EntityFrameworkCore;
using Starbender.FileClerk.BlobProviders;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Starbender.FileClerk.EntityFrameworkCore;

[ConnectionStringName(FileClerkDbProperties.ConnectionStringName)]
public class FileClerkDbContext : AbpDbContext<FileClerkDbContext>, IFileClerkDbContext
{
    public DbSet<FileClerkBlobProvider> BlobProviders => Set<FileClerkBlobProvider>();

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
