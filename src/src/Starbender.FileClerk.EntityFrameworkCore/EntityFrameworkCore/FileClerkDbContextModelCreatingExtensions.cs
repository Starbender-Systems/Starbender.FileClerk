using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Starbender.FileClerk.EntityFrameworkCore;

public static class FileClerkDbContextModelCreatingExtensions
{
    public static void ConfigureFileClerk(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure all entities here. Example:

        builder.Entity<Question>(b =>
        {
            //Configure table & schema name
            b.ToTable(FileClerkDbProperties.DbTablePrefix + "Questions", FileClerkDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

            //Relations
            b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

            //Indexes
            b.HasIndex(q => q.CreationTime);
        });
        */
    }
}
