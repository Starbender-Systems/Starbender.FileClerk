using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starbender.FileClerk.Demo.Authorization;
using Starbender.FileClerk.Demo.Security;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Security.Claims;
using Xunit;

namespace Starbender.FileClerk.Demo;

public sealed class DemoHttpApiTestFixture : IAsyncLifetime
{
    private WebApplication? _application;

    public HttpClient Client { get; private set; } = null!;

    public IServiceProvider ServiceProvider =>
        _application?.Services ??
        throw new InvalidOperationException("The HTTP API test host is not initialized.");

    public async Task InitializeAsync()
    {
        var contentRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../../../shared/Starbender.FileClerk.Demo.HttpApi.Host"));
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(DemoHttpApiHostModule).Assembly.GetName().Name,
            EnvironmentName = Environments.Development,
            ContentRootPath = contentRoot
        });

        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["App:SelfUrl"] = "http://localhost",
            ["App:DemoUrl"] = "http://localhost",
            ["App:CorsOrigins"] = "http://localhost",
            ["App:DisablePII"] = "true",
            ["AuthServer:Authority"] = "http://localhost",
            ["AuthServer:RequireHttpsMetadata"] = "false",
            ["AuthServer:SwaggerClientId"] = "AdministrativeAccessTests",
            ["StringEncryption:DefaultPassPhrase"] = "administrative-access-tests"
        });
        builder.Host.UseAutofac();

        await builder.AddApplicationAsync<DemoHttpApiTestModule>();

        _application = builder.Build();
        await _application.InitializeApplicationAsync();
        await _application.StartAsync();

        Client = _application.GetTestClient();

        using var scope = ServiceProvider.CreateScope();
        var permissionChecker = scope.ServiceProvider.GetRequiredService<IPermissionChecker>();
        if (permissionChecker is not TestPermissionChecker)
        {
            throw new InvalidOperationException(
                $"Expected {nameof(TestPermissionChecker)}, but resolved " +
                $"{permissionChecker.GetType().FullName}.");
        }
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (_application != null)
        {
            await _application.DisposeAsync();
        }
    }

    public async Task<object?> ArrangeAsync(AdministrativeAccessCase accessCase)
    {
        using var scope = ServiceProvider.CreateScope();
        var principalFactory = scope.ServiceProvider.GetRequiredService<TestPrincipalFactory>();
        var principalAccessor = scope.ServiceProvider.GetRequiredService<ICurrentPrincipalAccessor>();
        var principal = await principalFactory.CreateWithAllPermissionsAsync();

        using (principalAccessor.Change(principal))
        {
            return await accessCase.ArrangeAsync(scope.ServiceProvider);
        }
    }

    public async Task ApplyScenarioAsync(
        HttpRequestMessage request,
        AccessScenario scenario,
        string requiredPermission)
    {
        using var scope = ServiceProvider.CreateScope();
        var principalFactory = scope.ServiceProvider.GetRequiredService<TestPrincipalFactory>();
        var principal = await principalFactory.CreateAsync(scenario, requiredPermission);

        if (principal.Identity?.IsAuthenticated != true)
        {
            return;
        }

        request.Headers.Add(
            TestAuthenticationHandler.UserHeader,
            principal.FindFirst(AbpClaimTypes.UserId)!.Value);

        foreach (var permission in principal.FindAll(TestAuthorizationClaimTypes.Permission))
        {
            request.Headers.TryAddWithoutValidation(
                TestAuthenticationHandler.PermissionHeader,
                permission.Value);
        }
    }
}
