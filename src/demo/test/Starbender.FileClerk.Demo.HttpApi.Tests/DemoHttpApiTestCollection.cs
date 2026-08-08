using Xunit;

namespace Starbender.FileClerk.Demo;

[CollectionDefinition(Name)]
public class DemoHttpApiTestCollection : ICollectionFixture<DemoHttpApiTestFixture>
{
    public const string Name = "Demo HTTP API access tests";
}
