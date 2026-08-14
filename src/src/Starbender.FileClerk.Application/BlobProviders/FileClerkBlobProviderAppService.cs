using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Starbender.FileClerk.Configuration;
using Starbender.FileClerk.Features;
using Starbender.FileClerk.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;

namespace Starbender.FileClerk.BlobProviders;

[Authorize(FileClerkPermissions.ManageFileClerk)]
public class FileClerkBlobProviderAppService :
    FileClerkAppService,
    IFileClerkBlobProviderAppService
{
    private readonly IRepository<FileClerkBlobProvider, int> _repository;
    private readonly ICurrentTenant _currentTenant;
    private readonly IFeatureChecker _featureChecker;

    public FileClerkBlobProviderAppService(
        IRepository<FileClerkBlobProvider, int> repository,
        ICurrentTenant currentTenant,
        IFeatureChecker featureChecker)
    {
        _repository = repository;
        _currentTenant = currentTenant;
        _featureChecker = featureChecker;
    }

    public virtual async Task<IReadOnlyList<FileClerkBlobProviderDto>> GetListAsync()
    {
        var providers = await _repository.GetListAsync();

        if (_currentTenant.Id.HasValue)
        {
            var selectedIds = await GetEffectiveProviderIdsAsync();
            providers = providers
                .Where(provider => selectedIds.Contains(provider.Id))
                .ToList();
        }

        return providers
            .OrderBy(provider => provider.Name)
            .Select(Map)
            .ToList();
    }

    public virtual async Task<FileClerkBlobProviderDto> SetEnabledAsync(
        int id,
        SetFileClerkBlobProviderEnabledDto input)
    {
        if (_currentTenant.Id.HasValue)
        {
            throw new BusinessException(FileClerkErrorCodes.HostOnlyOperation);
        }

        var provider = await _repository.FindAsync(id)
                       ?? throw new BusinessException(FileClerkErrorCodes.ProviderNotFound)
                           .WithData("ProviderId", id);

        provider.SetEnabled(input.Enabled);
        await _repository.UpdateAsync(provider, autoSave: true);
        return Map(provider);
    }

    private async Task<HashSet<int>> GetEffectiveProviderIdsAsync()
    {
        var value = await _featureChecker.GetOrNullAsync(FileClerkFeatures.Providers);
        return FileClerkProviderSelection.TryParse(value, out var ids)
            ? ids.ToHashSet()
            : [];
    }

    private static FileClerkBlobProviderDto Map(FileClerkBlobProvider provider)
    {
        return new FileClerkBlobProviderDto
        {
            Id = provider.Id,
            Name = provider.Name,
            ImplementationType = provider.ImplementationType,
            ConfigurationSchema = provider.ConfigurationSchema,
            Enabled = provider.Enabled,
            CreationTime = provider.CreationTime,
            CreatorId = provider.CreatorId,
            LastModificationTime = provider.LastModificationTime,
            LastModifierId = provider.LastModifierId
        };
    }
}
