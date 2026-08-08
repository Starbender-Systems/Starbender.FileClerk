using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Starbender.FileClerk.Demo.Authorization;

public static class AdministrativeAccessCases
{
    public static IReadOnlyList<AdministrativeAccessCase> All { get; } =
    [
        .. CreateIdentityRoleCases(),
        .. CreateIdentityUserCases(),
        .. CreateTenantCases(),
        .. CreateSettingCases(),
        .. CreateFeatureCases()
    ];

    public static AdministrativeAccessCase Find(string id)
    {
        return All.Single(accessCase => accessCase.Id == id);
    }

    private static IEnumerable<AdministrativeAccessCase> CreateIdentityRoleCases()
    {
        yield return new(
            "identity.roles.get",
            IdentityPermissions.Roles.Default,
            ArrangeRoleAsync,
            (services, state) => Roles(services).GetAsync(Role(state).Id),
            state => Get($"/api/identity/roles/{Role(state).Id}"));

        yield return new(
            "identity.roles.get-all",
            IdentityPermissions.Roles.Default,
            NoArrangementAsync,
            (services, _) => Roles(services).GetAllListAsync(),
            _ => Get("/api/identity/roles/all"));

        yield return new(
            "identity.roles.list",
            IdentityPermissions.Roles.Default,
            NoArrangementAsync,
            (services, _) => Roles(services).GetListAsync(new GetIdentityRolesInput()),
            _ => Get("/api/identity/roles"));

        yield return new(
            "identity.roles.create",
            IdentityPermissions.Roles.Create,
            _ => Task.FromResult<object?>(new PayloadState<IdentityRoleCreateDto>(NewRoleCreateDto())),
            (services, state) => Roles(services).CreateAsync(Payload<IdentityRoleCreateDto>(state)),
            state => Json(HttpMethod.Post, "/api/identity/roles", Payload<IdentityRoleCreateDto>(state)));

        yield return new(
            "identity.roles.update",
            IdentityPermissions.Roles.Update,
            ArrangeRoleAsync,
            (services, state) => Roles(services).UpdateAsync(Role(state).Id, NewRoleUpdateDto(Role(state))),
            state => Json(HttpMethod.Put, $"/api/identity/roles/{Role(state).Id}", NewRoleUpdateDto(Role(state))));

        yield return new(
            "identity.roles.delete",
            IdentityPermissions.Roles.Delete,
            ArrangeRoleAsync,
            (services, state) => Roles(services).DeleteAsync(Role(state).Id),
            state => new HttpRequestMessage(HttpMethod.Delete, $"/api/identity/roles/{Role(state).Id}"));
    }

    private static IEnumerable<AdministrativeAccessCase> CreateIdentityUserCases()
    {
        yield return new(
            "identity.users.get",
            IdentityPermissions.Users.Default,
            ArrangeUserAsync,
            (services, state) => Users(services).GetAsync(User(state).Id),
            state => Get($"/api/identity/users/{User(state).Id}"));

        yield return new(
            "identity.users.list",
            IdentityPermissions.Users.Default,
            NoArrangementAsync,
            (services, _) => Users(services).GetListAsync(new GetIdentityUsersInput()),
            _ => Get("/api/identity/users"));

        yield return new(
            "identity.users.get-roles",
            IdentityPermissions.Users.Default,
            ArrangeUserAsync,
            (services, state) => Users(services).GetRolesAsync(User(state).Id),
            state => Get($"/api/identity/users/{User(state).Id}/roles"));

        yield return new(
            "identity.users.get-assignable-roles",
            IdentityPermissions.Users.Default,
            NoArrangementAsync,
            (services, _) => Users(services).GetAssignableRolesAsync(),
            _ => Get("/api/identity/users/assignable-roles"));

        yield return new(
            "identity.users.find-by-username",
            IdentityPermissions.Users.Default,
            ArrangeUserAsync,
            (services, state) => Users(services).FindByUsernameAsync(User(state).UserName),
            state => Get($"/api/identity/users/by-username/{WebUtility.UrlEncode(User(state).UserName)}"));

        yield return new(
            "identity.users.find-by-email",
            IdentityPermissions.Users.Default,
            ArrangeUserAsync,
            (services, state) => Users(services).FindByEmailAsync(User(state).Email),
            state => Get($"/api/identity/users/by-email/{WebUtility.UrlEncode(User(state).Email)}"));

        yield return new(
            "identity.users.find-by-id",
            IdentityPermissions.Users.Default,
            ArrangeUserAsync,
            (services, state) => Users(services).FindByIdAsync(User(state).Id),
            state => Get($"/api/identity/users/by-id/{User(state).Id}"));

        yield return new(
            "identity.users.create",
            IdentityPermissions.Users.Create,
            _ => Task.FromResult<object?>(new PayloadState<IdentityUserCreateDto>(NewUserCreateDto())),
            (services, state) => Users(services).CreateAsync(Payload<IdentityUserCreateDto>(state)),
            state => Json(HttpMethod.Post, "/api/identity/users", Payload<IdentityUserCreateDto>(state)));

        yield return new(
            "identity.users.update",
            IdentityPermissions.Users.Update,
            ArrangeUserAsync,
            (services, state) => Users(services).UpdateAsync(User(state).Id, NewUserUpdateDto(User(state))),
            state => Json(HttpMethod.Put, $"/api/identity/users/{User(state).Id}", NewUserUpdateDto(User(state))));

        yield return new(
            "identity.users.update-roles",
            IdentityPermissions.Users.Update,
            ArrangeUserAsync,
            (services, state) => Users(services).UpdateRolesAsync(
                User(state).Id,
                new IdentityUserUpdateRolesDto { RoleNames = [] }),
            state => Json(
                HttpMethod.Put,
                $"/api/identity/users/{User(state).Id}/roles",
                new IdentityUserUpdateRolesDto { RoleNames = [] }));

        yield return new(
            "identity.users.delete",
            IdentityPermissions.Users.Delete,
            ArrangeUserAsync,
            (services, state) => Users(services).DeleteAsync(User(state).Id),
            state => new HttpRequestMessage(HttpMethod.Delete, $"/api/identity/users/{User(state).Id}"));
    }

    private static IEnumerable<AdministrativeAccessCase> CreateTenantCases()
    {
        yield return new(
            "tenants.get",
            TenantManagementPermissions.Tenants.Default,
            ArrangeTenantAsync,
            (services, state) => Tenants(services).GetAsync(Tenant(state).Id),
            state => Get($"/api/multi-tenancy/tenants/{Tenant(state).Id}"));

        yield return new(
            "tenants.list",
            TenantManagementPermissions.Tenants.Default,
            NoArrangementAsync,
            (services, _) => Tenants(services).GetListAsync(new GetTenantsInput()),
            _ => Get("/api/multi-tenancy/tenants"));

        yield return new(
            "tenants.create",
            TenantManagementPermissions.Tenants.Create,
            _ => Task.FromResult<object?>(new PayloadState<TenantCreateDto>(NewTenantCreateDto())),
            (services, state) => Tenants(services).CreateAsync(Payload<TenantCreateDto>(state)),
            state => Json(HttpMethod.Post, "/api/multi-tenancy/tenants", Payload<TenantCreateDto>(state)));

        yield return new(
            "tenants.update",
            TenantManagementPermissions.Tenants.Update,
            ArrangeTenantAsync,
            (services, state) => Tenants(services).UpdateAsync(Tenant(state).Id, NewTenantUpdateDto(Tenant(state))),
            state => Json(
                HttpMethod.Put,
                $"/api/multi-tenancy/tenants/{Tenant(state).Id}",
                NewTenantUpdateDto(Tenant(state))));

        yield return new(
            "tenants.delete",
            TenantManagementPermissions.Tenants.Delete,
            ArrangeTenantAsync,
            (services, state) => Tenants(services).DeleteAsync(Tenant(state).Id),
            state => new HttpRequestMessage(HttpMethod.Delete, $"/api/multi-tenancy/tenants/{Tenant(state).Id}"));

        yield return new(
            "tenants.connection-string.get",
            TenantManagementPermissions.Tenants.ManageConnectionStrings,
            ArrangeTenantWithConnectionStringAsync,
            (services, state) => Tenants(services).GetDefaultConnectionStringAsync(Tenant(state).Id),
            state => Get($"/api/multi-tenancy/tenants/{Tenant(state).Id}/default-connection-string"));

        yield return new(
            "tenants.connection-string.update",
            TenantManagementPermissions.Tenants.ManageConnectionStrings,
            ArrangeTenantAsync,
            (services, state) => Tenants(services).UpdateDefaultConnectionStringAsync(
                Tenant(state).Id,
                "Data Source=authorization-tests"),
            state => new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/multi-tenancy/tenants/{Tenant(state).Id}/default-connection-string?defaultConnectionString=Data%20Source%3Dauthorization-tests"));

        yield return new(
            "tenants.connection-string.delete",
            TenantManagementPermissions.Tenants.ManageConnectionStrings,
            ArrangeTenantWithConnectionStringAsync,
            (services, state) => Tenants(services).DeleteDefaultConnectionStringAsync(Tenant(state).Id),
            state => new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/multi-tenancy/tenants/{Tenant(state).Id}/default-connection-string"));
    }

    private static IEnumerable<AdministrativeAccessCase> CreateSettingCases()
    {
        yield return new(
            "settings.emailing.get",
            SettingManagementPermissions.Emailing,
            NoArrangementAsync,
            (services, _) => EmailSettings(services).GetAsync(),
            _ => Get("/api/setting-management/emailing"));

        yield return new(
            "settings.emailing.update",
            SettingManagementPermissions.Emailing,
            ArrangeEmailSettingsAsync,
            (services, state) => EmailSettings(services).UpdateAsync(Payload<UpdateEmailSettingsDto>(state)),
            state => Json(
                HttpMethod.Post,
                "/api/setting-management/emailing",
                Payload<UpdateEmailSettingsDto>(state)));

        yield return new(
            "settings.emailing.send-test",
            SettingManagementPermissions.EmailingTest,
            _ => Task.FromResult<object?>(new PayloadState<SendTestEmailInput>(new SendTestEmailInput
            {
                SenderEmailAddress = "sender@example.test",
                TargetEmailAddress = "recipient@example.test",
                Subject = "Authorization test",
                Body = "Authorization test"
            })),
            (services, state) => EmailSettings(services).SendTestEmailAsync(Payload<SendTestEmailInput>(state)),
            state => Json(
                HttpMethod.Post,
                "/api/setting-management/emailing/send-test-email",
                Payload<SendTestEmailInput>(state)));

        yield return new(
            "settings.timezone.get",
            SettingManagementPermissions.TimeZone,
            NoArrangementAsync,
            (services, _) => TimeZones(services).GetAsync(),
            _ => Get("/api/setting-management/timezone"));

        yield return new(
            "settings.timezone.list",
            SettingManagementPermissions.TimeZone,
            NoArrangementAsync,
            (services, _) => TimeZones(services).GetTimezonesAsync(),
            _ => Get("/api/setting-management/timezone/timezones"));

        yield return new(
            "settings.timezone.update",
            SettingManagementPermissions.TimeZone,
            _ => Task.FromResult<object?>(new PayloadState<string>("UTC")),
            (services, state) => TimeZones(services).UpdateAsync(Payload<string>(state)),
            _ => new HttpRequestMessage(HttpMethod.Post, "/api/setting-management/timezone?timezone=UTC"));
    }

    private static IEnumerable<AdministrativeAccessCase> CreateFeatureCases()
    {
        foreach (var operation in CreateFeatureOperationCases(
                     "features.host",
                     FeatureManagementPermissions.ManageHostFeatures,
                     null))
        {
            yield return operation;
        }

        foreach (var operation in CreateTenantFeatureOperationCases())
        {
            yield return operation;
        }
    }

    private static IEnumerable<AdministrativeAccessCase> CreateFeatureOperationCases(
        string idPrefix,
        string permission,
        string? providerKey)
    {
        var query = FeatureQuery(providerKey);

        yield return new(
            $"{idPrefix}.get",
            permission,
            NoArrangementAsync,
            (services, _) => Features(services).GetAsync("T", providerKey!),
            _ => Get($"/api/feature-management/features{query}"));

        yield return new(
            $"{idPrefix}.update",
            permission,
            NoArrangementAsync,
            (services, _) => Features(services).UpdateAsync("T", providerKey!, EmptyFeatureUpdate()),
            _ => Json(HttpMethod.Put, $"/api/feature-management/features{query}", EmptyFeatureUpdate()));

        yield return new(
            $"{idPrefix}.delete",
            permission,
            NoArrangementAsync,
            (services, _) => Features(services).DeleteAsync("T", providerKey!),
            _ => new HttpRequestMessage(HttpMethod.Delete, $"/api/feature-management/features{query}"));
    }

    private static IEnumerable<AdministrativeAccessCase> CreateTenantFeatureOperationCases()
    {
        yield return TenantFeatureCase("get", (services, key) => Features(services).GetAsync("T", key));
        yield return TenantFeatureCase(
            "update",
            (services, key) => Features(services).UpdateAsync("T", key, EmptyFeatureUpdate()));
        yield return TenantFeatureCase("delete", (services, key) => Features(services).DeleteAsync("T", key));
    }

    private static AdministrativeAccessCase TenantFeatureCase(
        string operation,
        Func<IServiceProvider, string, Task> invoke)
    {
        return new AdministrativeAccessCase(
            $"features.tenant.{operation}",
            TenantManagementPermissions.Tenants.ManageFeatures,
            ArrangeTenantAsync,
            (services, state) => invoke(services, Tenant(state).Id.ToString()),
            state =>
            {
                var uri = $"/api/feature-management/features{FeatureQuery(Tenant(state).Id.ToString())}";
                return operation switch
                {
                    "get" => Get(uri),
                    "update" => Json(HttpMethod.Put, uri, EmptyFeatureUpdate()),
                    "delete" => new HttpRequestMessage(HttpMethod.Delete, uri),
                    _ => throw new InvalidOperationException($"Unknown feature operation: {operation}")
                };
            });
    }

    private static Task<object?> NoArrangementAsync(IServiceProvider _)
    {
        return Task.FromResult<object?>(null);
    }

    private static async Task<object?> ArrangeRoleAsync(IServiceProvider services)
    {
        return new RoleState(await Roles(services).CreateAsync(NewRoleCreateDto()));
    }

    private static async Task<object?> ArrangeUserAsync(IServiceProvider services)
    {
        return new UserState(await Users(services).CreateAsync(NewUserCreateDto()));
    }

    private static async Task<object?> ArrangeTenantAsync(IServiceProvider services)
    {
        return new TenantState(await Tenants(services).CreateAsync(NewTenantCreateDto()));
    }

    private static async Task<object?> ArrangeTenantWithConnectionStringAsync(IServiceProvider services)
    {
        var state = (TenantState)(await ArrangeTenantAsync(services))!;
        await Tenants(services).UpdateDefaultConnectionStringAsync(
            state.Tenant.Id,
            "Data Source=authorization-tests");
        return state;
    }

    private static async Task<object?> ArrangeEmailSettingsAsync(IServiceProvider services)
    {
        var current = await EmailSettings(services).GetAsync();
        return new PayloadState<UpdateEmailSettingsDto>(new UpdateEmailSettingsDto
        {
            SmtpHost = current.SmtpHost,
            SmtpPort = current.SmtpPort,
            SmtpUserName = current.SmtpUserName,
            SmtpPassword = current.SmtpPassword,
            SmtpDomain = current.SmtpDomain,
            SmtpEnableSsl = current.SmtpEnableSsl,
            SmtpUseDefaultCredentials = current.SmtpUseDefaultCredentials,
            DefaultFromAddress = current.DefaultFromAddress,
            DefaultFromDisplayName = current.DefaultFromDisplayName
        });
    }

    private static IdentityRoleCreateDto NewRoleCreateDto()
    {
        return new IdentityRoleCreateDto
        {
            Name = $"access-role-{Guid.NewGuid():N}",
            IsDefault = false,
            IsPublic = false
        };
    }

    private static IdentityRoleUpdateDto NewRoleUpdateDto(IdentityRoleDto role)
    {
        return new IdentityRoleUpdateDto
        {
            Name = $"{role.Name}-updated",
            ConcurrencyStamp = role.ConcurrencyStamp,
            IsDefault = role.IsDefault,
            IsPublic = role.IsPublic
        };
    }

    private static IdentityUserCreateDto NewUserCreateDto()
    {
        var suffix = Guid.NewGuid().ToString("N");
        return new IdentityUserCreateDto
        {
            UserName = $"access-user-{suffix}",
            Name = "Access",
            Surname = "Test",
            Email = $"access-{suffix}@example.test",
            PhoneNumber = null,
            Password = "1q2w3E*",
            IsActive = true,
            LockoutEnabled = true,
            RoleNames = []
        };
    }

    private static IdentityUserUpdateDto NewUserUpdateDto(IdentityUserDto user)
    {
        return new IdentityUserUpdateDto
        {
            UserName = user.UserName,
            Name = $"{user.Name}-updated",
            Surname = user.Surname,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Password = null,
            ConcurrencyStamp = user.ConcurrencyStamp,
            IsActive = user.IsActive,
            LockoutEnabled = user.LockoutEnabled,
            RoleNames = []
        };
    }

    private static TenantCreateDto NewTenantCreateDto()
    {
        var suffix = Guid.NewGuid().ToString("N");
        return new TenantCreateDto
        {
            Name = $"access-tenant-{suffix}",
            AdminEmailAddress = $"admin-{suffix}@example.test",
            AdminPassword = "1q2w3E*"
        };
    }

    private static TenantUpdateDto NewTenantUpdateDto(TenantDto tenant)
    {
        return new TenantUpdateDto
        {
            Name = $"{tenant.Name}-updated",
            ConcurrencyStamp = tenant.ConcurrencyStamp
        };
    }

    private static UpdateFeaturesDto EmptyFeatureUpdate()
    {
        return new UpdateFeaturesDto { Features = [] };
    }

    private static string FeatureQuery(string? providerKey)
    {
        return providerKey == null
            ? "?providerName=T"
            : $"?providerName=T&providerKey={WebUtility.UrlEncode(providerKey)}";
    }

    private static IIdentityRoleAppService Roles(IServiceProvider services) =>
        services.GetRequiredService<IIdentityRoleAppService>();

    private static IIdentityUserAppService Users(IServiceProvider services) =>
        services.GetRequiredService<IIdentityUserAppService>();

    private static ITenantAppService Tenants(IServiceProvider services) =>
        services.GetRequiredService<ITenantAppService>();

    private static IEmailSettingsAppService EmailSettings(IServiceProvider services) =>
        services.GetRequiredService<IEmailSettingsAppService>();

    private static ITimeZoneSettingsAppService TimeZones(IServiceProvider services) =>
        services.GetRequiredService<ITimeZoneSettingsAppService>();

    private static IFeatureAppService Features(IServiceProvider services) =>
        services.GetRequiredService<IFeatureAppService>();

    private static IdentityRoleDto Role(object? state) => ((RoleState)state!).Role;

    private static IdentityUserDto User(object? state) => ((UserState)state!).User;

    private static TenantDto Tenant(object? state) => ((TenantState)state!).Tenant;

    private static T Payload<T>(object? state) => ((PayloadState<T>)state!).Payload;

    private static HttpRequestMessage Get(string uri) => new(HttpMethod.Get, uri);

    private static HttpRequestMessage Json(HttpMethod method, string uri, object payload)
    {
        return new HttpRequestMessage(method, uri)
        {
            Content = JsonContent.Create(payload)
        };
    }
}
