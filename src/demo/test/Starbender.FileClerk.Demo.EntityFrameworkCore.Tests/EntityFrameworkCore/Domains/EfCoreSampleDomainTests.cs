using Starbender.FileClerk.Demo.Samples;
using Xunit;

namespace Starbender.FileClerk.Demo.EntityFrameworkCore.Domains;

[Collection(DemoTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<DemoEntityFrameworkCoreTestModule>
{

}
