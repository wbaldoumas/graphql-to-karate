using FluentAssertions;
using GraphQLToKarate.Library.Builders;
using GraphQLToKarate.Library.Converters;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Builders;

[TestFixture]
internal sealed class GraphQLToKarateConverterBuilderTests
{
    [Test]
    public void Build_builds_expected_converter()
    {
        // arrange
        var subjectUnderTest = new GraphQLToKarateConverterBuilder();

        // act
        var graphQLToKarateConverter = subjectUnderTest
            .Configure()
            .WithCustomScalarMapping(new Dictionary<string, string> { { "hello", "world" } })
            .Build();

        // assert
        graphQLToKarateConverter.Should().BeOfType<GraphQLToKarateConverter>();
    }
}