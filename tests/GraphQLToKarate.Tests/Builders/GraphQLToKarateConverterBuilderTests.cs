using FluentAssertions;
using GraphQLToKarate.Library.Builders;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Mappings;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Builders;

[TestFixture]
internal sealed class GraphQLToKarateConverterBuilderTests
{
    private ILogger<GraphQLToKarateConverter>? _mockLogger;

    [SetUp]
    public void SetUp() => _mockLogger = Substitute.For<ILogger<GraphQLToKarateConverter>>();

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Build_builds_expected_converter(bool populateCustomScalarMapping)
    {
        // arrange
        var customScalarMapping = new CustomScalarMapping(
            populateCustomScalarMapping
                ? new Dictionary<string, string> { { "hello", "world" } }
                : new Dictionary<string, string>()
        );

        var subjectUnderTest = new GraphQLToKarateConverterBuilder(_mockLogger!);

        // act
        var graphQLToKarateConverter = subjectUnderTest
            .Configure()
            .WithCustomScalarMapping(customScalarMapping)
            .WithBaseUrl("https://www.builder-test.com/graphql")
            .WithExcludeQueriesSetting(false)
            .WithQueryName("Hello")
            .WithTypeFilter(new HashSet<string> { "Test" })
            .WithOperationFilter(new HashSet<string> { "todos" })
            .Build();

        // assert
        graphQLToKarateConverter.Should().BeOfType<GraphQLToKarateConverter>();
    }
}