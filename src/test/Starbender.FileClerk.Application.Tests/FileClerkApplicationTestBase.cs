using Volo.Abp.Modularity;

namespace Starbender.FileClerk;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class FileClerkApplicationTestBase<TStartupModule> : FileClerkTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
