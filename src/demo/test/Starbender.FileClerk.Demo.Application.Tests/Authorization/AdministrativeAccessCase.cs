using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Starbender.FileClerk.Demo.Authorization;

public sealed record AdministrativeAccessCase(
    string Id,
    string RequiredPermission,
    Func<IServiceProvider, Task<object?>> ArrangeAsync,
    Func<IServiceProvider, object?, Task> InvokeApplicationAsync,
    Func<object?, HttpRequestMessage> CreateHttpRequest);
