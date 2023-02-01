using FluentAssertions;
using GraphQLToKarate.Library.Builders;
using GraphQLToKarate.Library.Converters;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Builders;

[TestFixture]
internal sealed class GraphQLToKarateConverterBuilderTests
{
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Build_builds_expected_converter(bool populateCustomScalarMapping)
    {
        // arrange
        var customScalarMapping = populateCustomScalarMapping
            ? new Dictionary<string, string> { { "hello", "world" } }
            : new Dictionary<string, string>();

        var subjectUnderTest = new GraphQLToKarateConverterBuilder();

        // act
        var graphQLToKarateConverter = subjectUnderTest
            .Configure()
            .WithCustomScalarMapping(customScalarMapping)
            .Build();

        // assert
        graphQLToKarateConverter.Should().BeOfType<GraphQLToKarateConverter>();
    }
}