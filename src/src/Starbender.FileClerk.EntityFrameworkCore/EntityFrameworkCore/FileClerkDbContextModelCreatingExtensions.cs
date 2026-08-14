using Microsoft.EntityFrameworkCore;
using Starbender.FileClerk.BlobProviders;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Starbender.FileClerk.EntityFrameworkCore;

public static class FileClerkDbContextModelCreatingExtensions
{
    public static void ConfigureFileClerk(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<FileClerkBlobProvider>(entity =>
        {
            entity.ToTable(
                FileClerkDbProperties.DbTablePrefix + "BlobProviders",
                FileClerkDbProperties.DbSchema);

            entity.ConfigureByConvention();

            entity.Property(provider => provider.Id).ValueGeneratedOnAdd();
            entity.Property(provider => provider.Name)
                .IsRequired()
                .HasMaxLength(FileClerkBlobProviderConsts.MaxNameLength);
            entity.Property(provider => provider.ImplementationType)
                .IsRequired()
                .HasMaxLength(FileClerkBlobProviderConsts.MaxImplementationTypeLength);
            entity.Property(provider => provider.ConfigurationSchema)
                .IsRequired()
                .HasMaxLength(FileClerkBlobProviderConsts.MaxConfigurationSchemaLength);
            entity.Property(provider => provider.Enabled)
                .IsRequired()
                .HasDefaultValue(false);

            entity.HasIndex(provider => provider.Name).IsUnique();
            entity.HasIndex(provider => provider.ImplementationType).IsUnique();
        });
    }
}
