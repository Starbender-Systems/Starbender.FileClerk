using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using Starbender.FileClerk.Demo.Authorization;
using Starbender.FileClerk.Demo.Security;
using Xunit;

namespace Starbender.FileClerk.Demo;

[Collection(DemoHttpApiTestCollection.Name)]
public class AdministrativeHttpApiAccessTests
{
    private readonly DemoHttpApiTestFixture _fixture;

    public AdministrativeHttpApiAccessTests(DemoHttpApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    public static IEnumerable<object[]> AccessCaseIds =>
        AdministrativeAccessCases.All.Select(accessCase => new object[] { accessCase.Id });

    [Theory]
    [MemberData(nameof(AccessCaseIds))]
    public async Task Api_Action_Should_Enforce_Access_Matrix(string accessCaseId)
    {
        var accessCase = AdministrativeAccessCases.Find(accessCaseId);
        var state = await _fixture.ArrangeAsync(accessCase);

        await AssertStatusAsync(accessCase, state, AccessScenario.Anonymous, HttpStatusCode.Unauthorized);
        await AssertStatusAsync(
            accessCase,
            state,
            AccessScenario.AuthenticatedWithoutPermission,
            HttpStatusCode.Forbidden);

        using var allowedRequest = accessCase.CreateHttpRequest(state);
        await _fixture.ApplyScenarioAsync(
            allowedRequest,
            AccessScenario.AuthenticatedWithPermission,
            accessCase.RequiredPermission);
        using var allowedResponse = await _fixture.Client.SendAsync(allowedRequest);
        var allowedBody = await allowedResponse.Content.ReadAsStringAsync();

        allowedResponse.IsSuccessStatusCode.ShouldBeTrue(
            $"Expected success for {accessCase.Id}, but received " +
            $"{(int)allowedResponse.StatusCode} {allowedResponse.StatusCode}: " +
            allowedBody);

        if (allowedRequest.Method == HttpMethod.Get || accessCase.Id.EndsWith(".create"))
        {
            allowedBody.ShouldNotBeNullOrWhiteSpace(
                $"Expected a response payload for successful read/create case {accessCase.Id}.");
        }
    }

    private async Task AssertStatusAsync(
        AdministrativeAccessCase accessCase,
        object? state,
        AccessScenario scenario,
        HttpStatusCode expectedStatus)
    {
        using var request = accessCase.CreateHttpRequest(state);
        await _fixture.ApplyScenarioAsync(request, scenario, accessCase.RequiredPermission);
        using var response = await _fixture.Client.SendAsync(request);

        response.StatusCode.ShouldBe(
            expectedStatus,
            $"Unexpected response for {accessCase.Id} in scenario {scenario}: " +
            await response.Content.ReadAsStringAsync());
    }
}
