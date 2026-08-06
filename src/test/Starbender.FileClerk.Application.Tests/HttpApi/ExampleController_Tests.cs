using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Starbender.FileClerk.Samples;

public class ExampleController_Tests
{
    private readonly ISampleAppService _sampleAppService;
    private readonly ExampleController _controller;

    public ExampleController_Tests()
    {
        _sampleAppService = Substitute.For<ISampleAppService>();
        _controller = new ExampleController(_sampleAppService);
    }

    [Fact]
    public async Task GetAsync_Should_Delegate_To_The_Application_Service()
    {
        var expected = new SampleDto { Value = 42 };
        _sampleAppService.GetAsync().Returns(expected);

        var result = await _controller.GetAsync();

        result.ShouldBeSameAs(expected);
        await _sampleAppService.Received(1).GetAsync();
        await _sampleAppService.DidNotReceive().GetAuthorizedAsync();
    }

    [Fact]
    public async Task GetAuthorizedAsync_Should_Delegate_To_The_Authorized_Application_Service_Method()
    {
        var expected = new SampleDto { Value = 42 };
        _sampleAppService.GetAuthorizedAsync().Returns(expected);

        var result = await _controller.GetAuthorizedAsync();

        result.ShouldBeSameAs(expected);
        await _sampleAppService.Received(1).GetAuthorizedAsync();
        await _sampleAppService.DidNotReceive().GetAsync();
    }

    [Fact]
    public void GetAuthorizedAsync_Should_Require_Authorization()
    {
        typeof(ExampleController)
            .GetMethod(nameof(ExampleController.GetAuthorizedAsync))!
            .GetCustomAttribute<AuthorizeAttribute>()
            .ShouldNotBeNull();
    }
}
