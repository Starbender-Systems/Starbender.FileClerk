using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;

namespace Starbender.FileClerk.Demo.BlazorWebAssembly.Blazor.Client;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        var baseUrl = builder.HostEnvironment.BaseAddress.TrimEnd('/');
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["App:SelfUrl"] = baseUrl,
            ["AuthServer:Authority"] = baseUrl,
            ["RemoteServices:Default:BaseUrl"] = baseUrl,
            ["RemoteServices:FileClerk:BaseUrl"] = baseUrl,
            ["RemoteServices:AbpAccountPublic:BaseUrl"] = baseUrl
        });

        var application = await builder.AddApplicationAsync<BlazorWebAssemblyBlazorClientModule>(options =>
        {
            options.UseAutofac();
        });

        var host = builder.Build();

        await application.InitializeApplicationAsync(host.Services);

        await host.RunAsync();
    }
}
