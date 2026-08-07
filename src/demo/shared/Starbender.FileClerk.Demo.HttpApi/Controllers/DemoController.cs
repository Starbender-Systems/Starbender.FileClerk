using Starbender.FileClerk.Demo.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Starbender.FileClerk.Demo.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DemoController : AbpControllerBase
{
    protected DemoController()
    {
        LocalizationResource = typeof(DemoResource);
    }
}
