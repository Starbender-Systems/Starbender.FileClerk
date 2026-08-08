using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;

namespace Starbender.FileClerk.Demo.Authorization;

internal sealed record RoleState(IdentityRoleDto Role);

internal sealed record UserState(IdentityUserDto User);

internal sealed record TenantState(TenantDto Tenant);

internal sealed record PayloadState<T>(T Payload);
