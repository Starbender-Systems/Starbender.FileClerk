using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.BlazoriseUI;

namespace Starbender.FileClerk.Demo.BlazorWebAssembly.Blazor.Client.Pages;

public partial class Index
{
    protected override void Dispose(bool disposing)
    {
        PageLayout.ShowToolbar = true;
    }
}
