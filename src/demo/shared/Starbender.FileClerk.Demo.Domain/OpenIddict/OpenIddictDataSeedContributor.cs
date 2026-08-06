using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.Uow;

namespace Starbender.FileClerk.Demo.OpenIddict;

/* Creates initial data that is needed to property run the application
 * and make client-to-server communication possible.
 */
public class OpenIddictDataSeedContributor : OpenIddictDataSeedContributorBase, IDataSeedContributor, ITransientDependency
{
    public OpenIddictDataSeedContributor(
        IConfiguration configuration,
        IOpenIddictApplicationRepository openIddictApplicationRepository,
        IAbpApplicationManager applicationManager,
        IOpenIddictScopeRepository openIddictScopeRepository,
        IOpenIddictScopeManager scopeManager)
        : base(configuration, openIddictApplicationRepository, applicationManager, openIddictScopeRepository, scopeManager)
    {
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await CreateScopesAsync();
        await CreateApplicationsAsync();
    }

    private async Task CreateScopesAsync()
    {
        await CreateScopesAsync(new OpenIddictScopeDescriptor
        {
            Name = "Demo",
            DisplayName = "Demo API",
            Resources = { "Demo" }
        });
    }

    private async Task CreateApplicationsAsync()
    {
        var commonScopes = new List<string> {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles,
            "Demo"
        };

        var configurationSection = Configuration.GetSection("OpenIddict:Applications");


        var angularClientId = configurationSection["FileClerkDemo_Angular:ClientId"];
        if (!angularClientId.IsNullOrWhiteSpace())
        {
            var angularRootUrl = configurationSection["FileClerkDemo_Angular:RootUrl"]!.TrimEnd('/');
            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: angularClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "FileClerk Angular Demo",
                secret: null,
                grantTypes: new List<string> {
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.GrantTypes.RefreshToken,
                    "LinkLogin",
                    "Impersonation"
                },
                scopes: commonScopes,
                redirectUris: new List<string> { angularRootUrl },
                postLogoutRedirectUris: new List<string> { angularRootUrl },
                clientUri: angularRootUrl,
                logoUri: "/images/clients/angular.svg"
            );
        }

        var blazorClientId = configurationSection["FileClerkDemo_BlazorWebAssembly:ClientId"];
        if (!blazorClientId.IsNullOrWhiteSpace())
        {
            var blazorRootUrl = configurationSection["FileClerkDemo_BlazorWebAssembly:RootUrl"]!.TrimEnd('/');
            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: blazorClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "FileClerk Blazor WebAssembly Demo",
                secret: null,
                grantTypes: new List<string> {
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.GrantTypes.RefreshToken
                },
                scopes: commonScopes,
                redirectUris: new List<string> { $"{blazorRootUrl}/authentication/login-callback" },
                postLogoutRedirectUris: new List<string> { $"{blazorRootUrl}/authentication/logout-callback" },
                clientUri: blazorRootUrl,
                logoUri: "/images/clients/blazor.svg"
            );
        }

        await CreateSwaggerClientAsync(configurationSection, "FileClerkDemo_AngularSwagger", commonScopes);
        await CreateSwaggerClientAsync(configurationSection, "FileClerkDemo_BlazorWebAssemblySwagger", commonScopes);
    }

    private async Task CreateSwaggerClientAsync(
        IConfigurationSection configurationSection,
        string configurationKey,
        List<string> commonScopes)
    {
        var clientId = configurationSection[$"{configurationKey}:ClientId"];
        if (!clientId.IsNullOrWhiteSpace())
        {
            var rootUrl = configurationSection[$"{configurationKey}:RootUrl"]!.TrimEnd('/');

            await CreateOrUpdateApplicationAsync(
                applicationType: OpenIddictConstants.ApplicationTypes.Web,
                name: clientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: $"{configurationKey} Swagger",
                secret: null,
                grantTypes: new List<string> { OpenIddictConstants.GrantTypes.AuthorizationCode, },
                scopes: commonScopes,
                redirectUris: new List<string> { $"{rootUrl}/swagger/oauth2-redirect.html" },
                clientUri: rootUrl.EnsureEndsWith('/') + "swagger",
                logoUri: "/images/clients/swagger.svg"
            );
        }
    }
}
