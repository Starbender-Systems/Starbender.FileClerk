using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Starbender.FileClerk.Demo.Security;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Abp.Testing;

namespace Starbender.FileClerk.Demo;

public abstract class DemoTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor =>
        GetRequiredService<ICurrentPrincipalAccessor>();

    protected TestPrincipalFactory TestPrincipalFactory =>
        GetRequiredService<TestPrincipalFactory>();

    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }

    protected override void BeforeAddApplication(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", false);
        builder.AddJsonFile("appsettings.secrets.json", true);
        services.ReplaceConfiguration(builder.Build());
    }

    protected virtual Task WithUnitOfWorkAsync(Func<Task> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task WithUnitOfWorkAsync(AbpUnitOfWorkOptions options, Func<Task> action)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin(options))
            {
                await action();

                await uow.CompleteAsync();
            }
        }
    }

    protected virtual Task<TResult> WithUnitOfWorkAsync<TResult>(Func<Task<TResult>> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task<TResult> WithUnitOfWorkAsync<TResult>(AbpUnitOfWorkOptions options, Func<Task<TResult>> func)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin(options))
            {
                var result = await func();
                await uow.CompleteAsync();
                return result;
            }
        }
    }

    protected virtual async Task RunAsAsync(
        AccessScenario scenario,
        string requiredPermission,
        Func<Task> action)
    {
        var principal = await TestPrincipalFactory.CreateAsync(scenario, requiredPermission);
        using (CurrentPrincipalAccessor.Change(principal))
        {
            await action();
        }
    }

    protected virtual async Task<TResult> RunAsAsync<TResult>(
        AccessScenario scenario,
        string requiredPermission,
        Func<Task<TResult>> action)
    {
        var principal = await TestPrincipalFactory.CreateAsync(scenario, requiredPermission);
        using (CurrentPrincipalAccessor.Change(principal))
        {
            return await action();
        }
    }

    protected virtual async Task RunWithAllPermissionsAsync(Func<Task> action)
    {
        var principal = await TestPrincipalFactory.CreateWithAllPermissionsAsync();
        using (CurrentPrincipalAccessor.Change(principal))
        {
            await action();
        }
    }

    protected virtual async Task<TResult> RunWithAllPermissionsAsync<TResult>(
        Func<Task<TResult>> action)
    {
        var principal = await TestPrincipalFactory.CreateWithAllPermissionsAsync();
        using (CurrentPrincipalAccessor.Change(principal))
        {
            return await action();
        }
    }

}
