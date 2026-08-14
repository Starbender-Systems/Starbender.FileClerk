using Microsoft.EntityFrameworkCore;
using Starbender.FileClerk.BlobProviders;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Starbender.FileClerk.EntityFrameworkCore;

[ConnectionStringName(FileClerkDbProperties.ConnectionStringName)]
public interface IFileClerkDbContext : IEfCoreDbContext
{
    DbSet<FileClerkBlobProvider> BlobProviders { get; }
}
