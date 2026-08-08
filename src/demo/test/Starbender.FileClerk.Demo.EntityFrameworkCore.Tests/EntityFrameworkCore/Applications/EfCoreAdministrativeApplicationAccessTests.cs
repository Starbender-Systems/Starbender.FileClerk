using Starbender.FileClerk.Demo.Authorization;
using Xunit;

namespace Starbender.FileClerk.Demo.EntityFrameworkCore.Applications;

[Collection(DemoTestConsts.CollectionDefinitionName)]
public class EfCoreAdministrativeApplicationAccessTests :
    AdministrativeApplicationAccessTests<DemoEntityFrameworkCoreTestModule>
{
}
