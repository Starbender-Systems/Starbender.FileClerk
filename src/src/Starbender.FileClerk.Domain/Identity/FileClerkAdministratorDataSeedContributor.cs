using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace Starbender.FileClerk.Identity;

public sealed class FileClerkAdministratorDataSeedContributor :
    IDataSeedContributor,
    ITransientDependency
{
    public const string RoleName = "FileClerkAdministrator";
    public const string DisplayName = "FileClerk:FileClerkAdministrator";
    private const string ManagePermissionName = "FileClerk.ManageFileClerk";

    private readonly IGuidGenerator _guidGenerator;
    private readonly ILookupNormalizer _lookupNormalizer;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IdentityRoleManager _roleManager;
    private readonly IdentityUserManager _userManager;
    private readonly IPermissionDataSeeder _permissionDataSeeder;

    public FileClerkAdministratorDataSeedContributor(
        IGuidGenerator guidGenerator,
        ILookupNormalizer lookupNormalizer,
        IIdentityRoleRepository roleRepository,
        IIdentityUserRepository userRepository,
        IdentityRoleManager roleManager,
        IdentityUserManager userManager,
        IPermissionDataSeeder permissionDataSeeder)
    {
        _guidGenerator = guidGenerator;
        _lookupNormalizer = lookupNormalizer;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _roleManager = roleManager;
        _userManager = userManager;
        _permissionDataSeeder = permissionDataSeeder;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (context.TenantId.HasValue)
        {
            return;
        }

        var normalizedRoleName = _lookupNormalizer.NormalizeName(RoleName);
        var role = await _roleRepository.FindByNormalizedNameAsync(normalizedRoleName);
        var roleWasCreated = role is null;

        if (roleWasCreated)
        {
            role = new IdentityRole(_guidGenerator.Create(), RoleName)
            {
                IsStatic = true,
                IsPublic = false
            };
            (await _roleManager.CreateAsync(role)).CheckErrors();
        }

        await _permissionDataSeeder.SeedAsync(
            RolePermissionValueProvider.ProviderName,
            RoleName,
            [ManagePermissionName]);

        if (!roleWasCreated)
        {
            return;
        }

        var adminUserName = (context[IdentityDataSeedContributor.AdminUserNamePropertyName] as string)
                            ?? IdentityDataSeedContributor.AdminUserNameDefaultValue;
        var admin = await _userRepository.FindByNormalizedUserNameAsync(
            _lookupNormalizer.NormalizeName(adminUserName));

        if (admin is not null)
        {
            (await _userManager.AddToRoleAsync(admin, RoleName)).CheckErrors();
        }
    }
}
