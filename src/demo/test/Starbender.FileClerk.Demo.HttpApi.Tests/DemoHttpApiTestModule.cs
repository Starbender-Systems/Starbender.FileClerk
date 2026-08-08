using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starbender.FileClerk.Demo.EntityFrameworkCore;
using Starbender.FileClerk.Demo.Security;
using Volo.Abp.AspNetCore.Security.Claims;
using Volo.Abp.AspNetCore.Mvc.Libs;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;

namespace Starbender.FileClerk.Demo;

[DependsOn(
    typeof(DemoHttpApiHostModule),
    typeof(DemoEntityFrameworkCoreTestModule)
)]
public class DemoHttpApiTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpMvcLibsOptions>(options =>
        {
            options.CheckLibs = false;
        });

        context.Services.Replace(
            ServiceDescriptor.Transient<IPermissionChecker, TestPermissionChecker>());

        context.Services.Replace(
            ServiceDescriptor.Singleton<ICurrentPrincipalAccessor, HttpContextCurrentPrincipalAccessor>());
        Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = false;
        });

        context.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.AuthenticationScheme;
                options.DefaultChallengeScheme = TestAuthenticationHandler.AuthenticationScheme;
                options.DefaultForbidScheme = TestAuthenticationHandler.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.AuthenticationScheme,
                _ => { });
    }
}
