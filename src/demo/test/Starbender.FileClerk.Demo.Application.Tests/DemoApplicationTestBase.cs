using Volo.Abp.Modularity;

namespace Starbender.FileClerk.Demo;

public abstract class DemoApplicationTestBase<TStartupModule> : DemoTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
