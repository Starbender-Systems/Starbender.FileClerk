using Starbender.FileClerk.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Starbender.FileClerk.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class FileClerkPageModel : AbpPageModel
{
    protected FileClerkPageModel()
    {
        LocalizationResourceType = typeof(FileClerkResource);
        ObjectMapperContext = typeof(FileClerkWebModule);
    }
}
