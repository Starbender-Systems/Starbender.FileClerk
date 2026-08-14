using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Starbender.FileClerk.BlobProviders;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;

namespace Starbender.FileClerk.Features;

public sealed class FileClerkFeatureValueProvider : FeatureValueProvider
{
    public const string ProviderName = "FileClerk";
    private const string TenantProviderName = TenantFeatureValueProvider.ProviderName;

    private readonly ICurrentTenant _currentTenant;
    private readonly IRepository<FileClerkBlobProvider, int> _providerRepository;
    private readonly ILogger<FileClerkFeatureValueProvider> _logger;

    public override string Name => ProviderName;

    public FileClerkFeatureValueProvider(
        IFeatureStore featureStore,
        ICurrentTenant currentTenant,
        IRepository<FileClerkBlobProvider, int> providerRepository,
        ILogger<FileClerkFeatureValueProvider> logger)
        : base(featureStore)
    {
        _currentTenant = currentTenant;
        _providerRepository = providerRepository;
        _logger = logger;
    }

    public override async Task<string?> GetOrNullAsync(FeatureDefinition feature)
    {
        if (feature.Name == FileClerkFeatures.Enabled)
        {
            return await GetEnabledAsync();
        }

        if (feature.Name == FileClerkFeatures.Providers)
        {
            return await GetProvidersAsync();
        }

        return null;
    }

    private async Task<string> GetEnabledAsync()
    {
        var hostEnabled = await ReadEnabledAsync(providerKey: null);
        if (!_currentTenant.Id.HasValue)
        {
            return hostEnabled ? "true" : "false";
        }

        if (!hostEnabled)
        {
            return "false";
        }

        return await ReadEnabledAsync(_currentTenant.Id.Value.ToString())
            ? "true"
            : "false";
    }

    private async Task<string> GetProvidersAsync()
    {
        var enabledIds = (await _providerRepository.GetListAsync(provider => provider.Enabled))
            .Select(provider => provider.Id)
            .ToHashSet();

        var hostIds = await ReadProviderIdsAsync(providerKey: null);
        hostIds.IntersectWith(enabledIds);

        if (!_currentTenant.Id.HasValue)
        {
            return FileClerkProviderSelection.Serialize(hostIds);
        }

        var tenantIds = await ReadProviderIdsAsync(_currentTenant.Id.Value.ToString());
        tenantIds.IntersectWith(hostIds);
        return FileClerkProviderSelection.Serialize(tenantIds);
    }

    private async Task<bool> ReadEnabledAsync(string? providerKey)
    {
        var value = await FeatureStore.GetOrNullAsync(
            FileClerkFeatures.Enabled,
            TenantProviderName,
            providerKey);

        if (bool.TryParse(value, out var enabled))
        {
            return enabled;
        }

        if (value is not null)
        {
            _logger.LogWarning(
                "Ignoring invalid persisted FileClerk feature value {FeatureName}={FeatureValue} for provider key {ProviderKey}.",
                FileClerkFeatures.Enabled,
                value,
                providerKey ?? "<host>");
        }

        return false;
    }

    private async Task<HashSet<int>> ReadProviderIdsAsync(string? providerKey)
    {
        var value = await FeatureStore.GetOrNullAsync(
            FileClerkFeatures.Providers,
            TenantProviderName,
            providerKey);

        if (FileClerkProviderSelection.TryParse(value, out var providerIds))
        {
            return providerIds.ToHashSet();
        }

        if (value is not null)
        {
            _logger.LogWarning(
                "Ignoring invalid legacy FileClerk provider selection {FeatureValue} for provider key {ProviderKey}.",
                value,
                providerKey ?? "<host>");
        }

        return [];
    }
}
