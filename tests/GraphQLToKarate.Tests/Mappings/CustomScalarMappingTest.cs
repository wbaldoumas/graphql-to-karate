using FluentAssertions;
using GraphQLToKarate.Library.Mappings;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Mappings;

[TestFixture]
public class CustomScalarMappingTests
{
    [TestCase("String", "string")]
    [TestCase("Int", "int")]
    [TestCase("Boolean", "boolean")]
    public void TryGetKarateType_should_return_true_and_karate_type_when_mapping_exists(
        string graphQLType,
        string expectedKarateType)
    {
        // arrange
        var customScalarMapping = new CustomScalarMapping(
            new Dictionary<string, string>
            {
                { "String", "string" },
                { "Int", "int" },
                { "Boolean", "boolean" }
            }
        );

        // act
        var result = customScalarMapping.TryGetKarateType(graphQLType, out var karateType);

        // assert
        customScalarMapping.Any().Should().BeTrue();
        result.Should().BeTrue();
        karateType.Should().Be(expectedKarateType);
    }

    [TestCase("Float")]
    [TestCase("ID")]
    public void TryGetKarateType_should_return_false_and_null_karate_type_when_mapping_does_not_exist(
        string graphQLType)
    {
        // arrange
        var customScalarMapping = new CustomScalarMapping(
            new Dictionary<string, string>
            {
                { "String", "string" },
                { "Int", "int" },
                { "Boolean", "boolean" }
            }
        );

        // act
        var result = customScalarMapping.TryGetKarateType(graphQLType, out var karateType);

        // assert
        customScalarMapping.Any().Should().BeTrue();
        result.Should().BeFalse();
        karateType.Should().BeNull();
    }

    [Test]
    public void ToString_generates_expected_string()
    {
        // arrange
        var customScalarMapping = new CustomScalarMapping(
            new Dictionary<string, string>
            {
                { "String", "string" },
                { "Int", "int" },
                { "Boolean", "boolean" }
            }
        );

        // act
        var result = customScalarMapping.ToString();

        // assert
        result.Should().Be("String:string,Int:int,Boolean:boolean");
    }
}