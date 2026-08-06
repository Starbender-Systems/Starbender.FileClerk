using Starbender.FileClerk.Demo.Localization;
using Volo.Abp.Application.Services;

namespace Starbender.FileClerk.Demo;

/* Inherit your application services from this class.
 */
public abstract class DemoAppService : ApplicationService
{
    protected DemoAppService()
    {
        LocalizationResource = typeof(DemoResource);
    }
}
