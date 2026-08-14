using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Starbender.FileClerk.BlobProviders;

public sealed class FileClerkBlobProviderDataSeedContributor :
    IDataSeedContributor,
    ITransientDependency
{
    public static readonly string[] ImplementationTypes =
    [
        "Volo.Abp.BlobStoring.Memory",
        "Volo.Abp.BlobStoring.FileSystem",
        "Volo.Abp.BlobStoring.Database.EntityFrameworkCore",
        "Volo.Abp.BlobStoring.Database.MongoDB",
        "Volo.Abp.BlobStoring.Azure",
        "Volo.Abp.BlobStoring.Aliyun",
        "Volo.Abp.BlobStoring.Minio",
        "Volo.Abp.BlobStoring.Aws",
        "Volo.Abp.BlobStoring.Google",
        "Volo.Abp.BlobStoring.Bunny"
    ];

    private const string DefaultConfigurationSchema = "{\"connectionString\":\"\"}";

    private readonly IRepository<FileClerkBlobProvider, int> _repository;

    public FileClerkBlobProviderDataSeedContributor(
        IRepository<FileClerkBlobProvider, int> repository)
    {
        _repository = repository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (context.TenantId.HasValue)
        {
            return;
        }

        var existing = await _repository.GetListAsync();
        var implementationTypes = existing
            .Select(provider => provider.ImplementationType)
            .ToHashSet(StringComparer.Ordinal);
        var names = existing
            .Select(provider => provider.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var implementationType in ImplementationTypes)
        {
            if (implementationTypes.Contains(implementationType))
            {
                continue;
            }

            var name = GetUniqueName(implementationType, names);
            await _repository.InsertAsync(
                new FileClerkBlobProvider(
                    name,
                    implementationType,
                    DefaultConfigurationSchema),
                autoSave: true);

            implementationTypes.Add(implementationType);
            names.Add(name);
        }
    }

    internal static string GetUniqueName(
        string implementationType,
        ISet<string> existingNames)
    {
        var parts = implementationType.Split('.');
        for (var partCount = 1; partCount <= parts.Length; partCount++)
        {
            var candidate = string.Join('.', parts[^partCount..]);
            if (!existingNames.Contains(candidate))
            {
                return candidate;
            }
        }

        for (var suffix = 2; ; suffix++)
        {
            var candidate = $"{implementationType}.{suffix}";
            if (!existingNames.Contains(candidate))
            {
                return candidate;
            }
        }
    }
}
