using Volo.Abp.Modularity;

namespace Starbender.FileClerk;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class FileClerkDomainTestBase<TStartupModule> : FileClerkTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
