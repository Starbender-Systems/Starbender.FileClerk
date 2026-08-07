using System;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using Starbender.FileClerk.Demo.EntityFrameworkCore;
using Starbender.FileClerk.Demo.Localization;
using Starbender.FileClerk.Demo.MultiTenancy;
using Starbender.FileClerk.Demo.Web.HealthChecks;
using Starbender.FileClerk.Demo.Web.Menus;
using Starbender.FileClerk.Web;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.Security.Claims;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Studio;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace Starbender.FileClerk.Demo.Web;

[DependsOn(
    typeof(FileClerkWebModule),
    typeof(DemoApplicationModule),
    typeof(DemoEntityFrameworkCoreModule),
    typeof(DemoHttpApiModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpPermissionManagementWebModule),
    typeof(AbpFeatureManagementWebModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpAspNetCoreSerilogModule)
)]
public class DemoWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(DemoResource),
                typeof(DemoDomainModule).Assembly,
                typeof(DemoDomainSharedModule).Assembly,
                typeof(DemoApplicationModule).Assembly,
                typeof(DemoApplicationContractsModule).Assembly,
                typeof(DemoWebModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("Demo");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddProductionEncryptionAndSigningCertificate(
                    "openiddict.pfx",
                    configuration["AuthServer:CertificatePassPhrase"]!
                );
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        }

        if (!configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata"))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            });

            Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        if (hostingEnvironment.IsDevelopment())
        {
            context.Services.AddRazorPages().AddRazorRuntimeCompilation();
        }

        ConfigureStudio(hostingEnvironment);
        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureBundles(hostingEnvironment);
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureConventionalControllers();
        ConfigureHealthChecks(context);
        ConfigureSwagger(context.Services);
        ConfigureMenu();
    }

    private void ConfigureStudio(IHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsProduction())
        {
            Configure<AbpStudioClientOptions>(options => options.IsLinkEnabled = false);
        }
    }

    private static void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(
            OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme
        );
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(
                configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>()
            );
        });
    }

    private void ConfigureBundles(IHostEnvironment hostingEnvironment)
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                BasicThemeBundles.Styles.Global,
                bundle => bundle.AddFiles("/global-styles.css")
            );
            options.ScriptBundles.Configure(
                BasicThemeBundles.Scripts.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-scripts.js");
                    if (hostingEnvironment.IsDevelopment())
                    {
                        bundle.AddFiles("/dev-login-helper.js");
                    }
                }
            );
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<DemoDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath, "..", "..", "shared", "Starbender.FileClerk.Demo.Domain.Shared")
                );
                options.FileSets.ReplaceEmbeddedByPhysical<DemoDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath, "..", "..", "shared", "Starbender.FileClerk.Demo.Domain")
                );
                options.FileSets.ReplaceEmbeddedByPhysical<DemoApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath, "..", "..", "shared", "Starbender.FileClerk.Demo.Application.Contracts")
                );
                options.FileSets.ReplaceEmbeddedByPhysical<DemoApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath, "..", "..", "shared", "Starbender.FileClerk.Demo.Application")
                );
                options.FileSets.ReplaceEmbeddedByPhysical<DemoWebModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(DemoApplicationModule).Assembly);
        });
    }

    private static void ConfigureHealthChecks(ServiceConfigurationContext context)
    {
        context.Services.AddDemoWebHealthChecks();
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo MVC API", Version = "v1" });
            options.DocInclusionPredicate((_, _) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
    }

    private void ConfigureMenu()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new DemoWebMenuContributor());
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.UseRouting();
        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();
        app.UseAbpSecurityHeaders();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo MVC API");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
