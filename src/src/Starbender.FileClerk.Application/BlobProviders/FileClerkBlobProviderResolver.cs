using System;
using System.Linq;
using System.Threading.Tasks;
using Starbender.FileClerk.Features;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Features;

namespace Starbender.FileClerk.BlobProviders;

public sealed class FileClerkBlobProviderResolver :
    IFileClerkBlobProviderResolver,
    ITransientDependency
{
    private readonly IRepository<FileClerkBlobProvider, int> _repository;
    private readonly IFeatureChecker _featureChecker;
    private readonly IBlobProviderSelector _blobProviderSelector;

    public FileClerkBlobProviderResolver(
        IRepository<FileClerkBlobProvider, int> repository,
        IFeatureChecker featureChecker,
        IBlobProviderSelector blobProviderSelector)
    {
        _repository = repository;
        _featureChecker = featureChecker;
        _blobProviderSelector = blobProviderSelector;
    }

    public async Task<IBlobProvider> ResolveAsync(int providerId, string containerName)
    {
        var provider = await _repository.FindAsync(providerId)
                       ?? throw new BusinessException(FileClerkErrorCodes.ProviderNotFound)
                           .WithData("ProviderId", providerId);

        if (!provider.Enabled)
        {
            throw new BusinessException(FileClerkErrorCodes.ProviderDisabled)
                .WithData("ProviderId", providerId);
        }

        var fileClerkEnabled = string.Equals(
            await _featureChecker.GetOrNullAsync(FileClerkFeatures.Enabled),
            "true",
            StringComparison.OrdinalIgnoreCase);
        if (!fileClerkEnabled)
        {
            throw new BusinessException(FileClerkErrorCodes.HostFileClerkDisabled);
        }

        var selectedValue = await _featureChecker.GetOrNullAsync(FileClerkFeatures.Providers);
        var selected = FileClerkProviderSelection.TryParse(selectedValue, out var ids)
            ? ids
            : [];

        if (!selected.Contains(providerId))
        {
            throw new BusinessException(FileClerkErrorCodes.ProviderNotSelected)
                .WithData("ProviderId", providerId);
        }

        return _blobProviderSelector.Get(containerName);
    }
}
