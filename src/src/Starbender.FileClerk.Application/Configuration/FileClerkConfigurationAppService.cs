using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Starbender.FileClerk.BlobProviders;
using Starbender.FileClerk.Features;
using Starbender.FileClerk.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;

namespace Starbender.FileClerk.Configuration;

[Authorize(FileClerkPermissions.ManageFileClerk)]
public class FileClerkConfigurationAppService :
    FileClerkAppService,
    IFileClerkConfigurationAppService
{
    private const string FeatureProviderName = TenantFeatureValueProvider.ProviderName;

    private readonly IRepository<FileClerkBlobProvider, int> _providerRepository;
    private readonly ICurrentTenant _currentTenant;
    private readonly IFeatureChecker _featureChecker;
    private readonly IFeatureStore _featureStore;
    private readonly IFeatureManager _featureManager;

    public FileClerkConfigurationAppService(
        IRepository<FileClerkBlobProvider, int> providerRepository,
        ICurrentTenant currentTenant,
        IFeatureChecker featureChecker,
        IFeatureStore featureStore,
        IFeatureManager featureManager)
    {
        _providerRepository = providerRepository;
        _currentTenant = currentTenant;
        _featureChecker = featureChecker;
        _featureStore = featureStore;
        _featureManager = featureManager;
    }

    public virtual async Task<FileClerkConfigurationDto> GetAsync()
    {
        var availableProviders = await GetAvailableProvidersAsync();
        var effectiveProviderIds = await GetEffectiveProviderIdsAsync();

        return new FileClerkConfigurationDto
        {
            EffectiveEnabled = await GetEffectiveEnabledAsync(),
            SelectedProviderIds = effectiveProviderIds,
            AvailableProviders = availableProviders
                .Select(provider => new FileClerkProviderChoiceDto
                {
                    Id = provider.Id,
                    Name = provider.Name
                })
                .ToList(),
            CanManageProviderRegistry = !_currentTenant.Id.HasValue
        };
    }

    public virtual async Task<FileClerkConfigurationDto> UpdateAsync(
        UpdateFileClerkConfigurationDto input)
    {
        Check.NotNull(input, nameof(input));
        var requestedIds = input.ProviderIds?.ToArray() ?? [];

        if (requestedIds.Length != requestedIds.Distinct().Count())
        {
            throw new BusinessException(FileClerkErrorCodes.DuplicateProviderSelection);
        }

        if (_currentTenant.Id.HasValue && !await GetHostEnabledAsync())
        {
            throw new BusinessException(FileClerkErrorCodes.HostFileClerkDisabled);
        }

        var allProviders = await _providerRepository.GetListAsync();
        var knownIds = allProviders.Select(provider => provider.Id).ToHashSet();
        var unknownId = requestedIds.Cast<int?>()
            .FirstOrDefault(id => !knownIds.Contains(id!.Value));
        if (unknownId.HasValue)
        {
            throw new BusinessException(FileClerkErrorCodes.ProviderNotFound)
                .WithData("ProviderId", unknownId.Value);
        }

        var availableProviders = await GetAvailableProvidersAsync();
        var availableIds = availableProviders.Select(provider => provider.Id).ToHashSet();
        var outOfScopeId = requestedIds.Cast<int?>()
            .FirstOrDefault(id => !availableIds.Contains(id!.Value));
        if (outOfScopeId.HasValue)
        {
            var provider = allProviders.Single(item => item.Id == outOfScopeId.Value);
            var errorCode = provider.Enabled
                ? FileClerkErrorCodes.ProviderSelectionOutOfScope
                : FileClerkErrorCodes.ProviderDisabled;
            throw new BusinessException(errorCode)
                .WithData("ProviderId", outOfScopeId.Value);
        }

        var providerKey = _currentTenant.Id?.ToString();
        var persistedIds = await ReadPersistedProviderIdsAsync(providerKey);
        persistedIds.RemoveWhere(availableIds.Contains);
        persistedIds.UnionWith(requestedIds);

        await _featureManager.SetAsync(
            FileClerkFeatures.Enabled,
            input.Enabled ? "true" : "false",
            FeatureProviderName,
            providerKey,
            forceToSet: true);
        await _featureManager.SetAsync(
            FileClerkFeatures.Providers,
            FileClerkProviderSelection.Serialize(persistedIds),
            FeatureProviderName,
            providerKey,
            forceToSet: true);

        return await GetAsync();
    }

    private async Task<List<FileClerkBlobProvider>> GetAvailableProvidersAsync()
    {
        var providers = (await _providerRepository.GetListAsync(provider => provider.Enabled))
            .OrderBy(provider => provider.Name)
            .ToList();

        if (!_currentTenant.Id.HasValue)
        {
            return providers;
        }

        if (!await GetHostEnabledAsync())
        {
            return [];
        }

        var hostIds = await ReadPersistedProviderIdsAsync(providerKey: null);
        return providers.Where(provider => hostIds.Contains(provider.Id)).ToList();
    }

    private async Task<IReadOnlyList<int>> GetEffectiveProviderIdsAsync()
    {
        var value = await _featureChecker.GetOrNullAsync(FileClerkFeatures.Providers);
        return FileClerkProviderSelection.TryParse(value, out var ids) ? ids : [];
    }

    private async Task<bool> GetEffectiveEnabledAsync()
    {
        return string.Equals(
            await _featureChecker.GetOrNullAsync(FileClerkFeatures.Enabled),
            "true",
            StringComparison.OrdinalIgnoreCase);
    }

    private async Task<bool> GetHostEnabledAsync()
    {
        return bool.TryParse(
                   await _featureStore.GetOrNullAsync(
                       FileClerkFeatures.Enabled,
                       FeatureProviderName,
                       providerKey: null),
                   out var enabled) &&
               enabled;
    }

    private async Task<HashSet<int>> ReadPersistedProviderIdsAsync(string? providerKey)
    {
        var value = await _featureStore.GetOrNullAsync(
            FileClerkFeatures.Providers,
            FeatureProviderName,
            providerKey);
        return FileClerkProviderSelection.TryParse(value, out var ids)
            ? ids.ToHashSet()
            : [];
    }
}
