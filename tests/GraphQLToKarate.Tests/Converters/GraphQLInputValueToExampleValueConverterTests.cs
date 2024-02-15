using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Exceptions;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Tests.Mocks;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLInputValueToExampleValueConverterTests
{
    private IGraphQLScalarToExampleValueConverter? _mockGraphQLScalarToExampleValueConverter;
    private IGraphQLDocumentAdapter? _mockGraphQLDocumentAdapter;
    private IGraphQLInputValueToExampleValueConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLScalarToExampleValueConverter = Substitute.For<IGraphQLScalarToExampleValueConverter>();
        _mockGraphQLDocumentAdapter = Substitute.For<IGraphQLDocumentAdapter>();
        _subjectUnderTest = new GraphQLInputValueToExampleValueConverter(_mockGraphQLScalarToExampleValueConverter);
    }

    [Test]
    [TestCaseSource(nameof(NonInputTypeTestCases))]
    public void Convert_returns_expected_example_value_for_non_input_types(
        GraphQLInputValueDefinition graphQLInputValueDefinition,
        string scalarExampleValueReturn,
        string expectedExampleValue)
    {
        // arrange
        _mockGraphQLScalarToExampleValueConverter!
            .Convert(Arg.Any<GraphQLNamedType>(), Arg.Any<IGraphQLDocumentAdapter>())
            .Returns(scalarExampleValueReturn);

        _mockGraphQLDocumentAdapter!
            .IsGraphQLInputObjectTypeDefinition(Arg.Any<string>())
            .Returns(false);

        // act
        var exampleValue = _subjectUnderTest!.Convert(
            graphQLInputValueDefinition,
            _mockGraphQLDocumentAdapter!
        );

        // assert
        exampleValue.Should().Be(expectedExampleValue);
    }

    private static IEnumerable<TestCaseData> NonInputTypeTestCases
    {
        get
        {
            const string exampleValue = "\"exampleValue\"";

            yield return new TestCaseData(
                new GraphQLInputValueDefinition(
                    new GraphQLName("exampleValue"),
                    new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                ),
                exampleValue,
                exampleValue
            );

            yield return new TestCaseData(
                new GraphQLInputValueDefinition(
                    new GraphQLName("exampleValue"),
                    new GraphQLNonNullType(
                        new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                    )
                ),
                exampleValue,
                exampleValue
            );

            yield return new TestCaseData(
                new GraphQLInputValueDefinition(
                    new GraphQLName("exampleValue"),
                    new GraphQLListType(
                        new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                    )
                ),
                exampleValue,
                $"[ {exampleValue} ]"
            );
        }
    }

    [Test]
    public void Convert_returns_expected_example_value_for_input_type()
    {
        // arrange
        var nestedGraphQLInputObjectTypeDefinition = new GraphQLInputObjectTypeDefinition(
            new GraphQLName("TestNestedInputObjectTypeDefinition"))
        {
            Fields = new GraphQLInputFieldsDefinition(
                [
                    new GraphQLInputValueDefinition(
                        new GraphQLName("nestedValue"),
                        new GraphQLNamedType(new GraphQLName(GraphQLToken.Int))
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("nestedValue2"),
                        new GraphQLListType(
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.Int))
                        )
                    )
                ]
            )
        };

        var graphQLInputObjectTypeDefinition = new GraphQLInputObjectTypeDefinition(
            new GraphQLName("TestInputObjectTypeDefinition"))
        {
            Fields = new GraphQLInputFieldsDefinition(
                [
                    new GraphQLInputValueDefinition(
                        new GraphQLName("exampleValue"),
                        new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("exampleValue2"),
                        new GraphQLListType(
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.Int))
                        )
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("exampleValue3"),
                        new GraphQLNamedType(new GraphQLName(GraphQLToken.Float))
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("exampleValue4"),
                        new GraphQLNamedType(new GraphQLName(GraphQLToken.Boolean))
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("exampleValue5"),
                        new GraphQLNonNullType(
                            new GraphQLNamedType(new GraphQLName(nestedGraphQLInputObjectTypeDefinition.Name))
                        )
                    )
                ]
            )
        };

        var graphQLInputValueDefinition = new GraphQLInputValueDefinition(
            new GraphQLName("exampleInput"),
            new GraphQLNamedType(new GraphQLName(graphQLInputObjectTypeDefinition.NameValue()))
        );

        _mockGraphQLDocumentAdapter!
            .IsGraphQLInputObjectTypeDefinition(graphQLInputObjectTypeDefinition.NameValue())
            .Returns(true);

        _mockGraphQLDocumentAdapter!
            .IsGraphQLInputObjectTypeDefinition(nestedGraphQLInputObjectTypeDefinition.NameValue())
            .Returns(true);

        _mockGraphQLDocumentAdapter
            .GetGraphQLInputObjectTypeDefinition(graphQLInputObjectTypeDefinition.NameValue())
            .Returns(graphQLInputObjectTypeDefinition);

        _mockGraphQLDocumentAdapter
            .GetGraphQLInputObjectTypeDefinition(nestedGraphQLInputObjectTypeDefinition.NameValue())
            .Returns(nestedGraphQLInputObjectTypeDefinition);

        const string exampleStringValue = "\"an example value\"";
        const string exampleIntValue = "1";
        const string exampleFloatValue = "1.0";
        const string exampleBooleanValue = "true";

        _mockGraphQLScalarToExampleValueConverter!
            .Convert(
                Arg.Is<GraphQLType>(
                    graphQLType => (graphQLType as GraphQLNamedType)!.NameValue() == GraphQLToken.String),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(exampleStringValue);

        _mockGraphQLScalarToExampleValueConverter!
            .Convert(
                Arg.Is<GraphQLType>(graphQLType => (graphQLType as GraphQLNamedType)!.NameValue() == GraphQLToken.Int),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(exampleIntValue);

        _mockGraphQLScalarToExampleValueConverter!
            .Convert(
                Arg.Is<GraphQLType>(graphQLType =>
                    (graphQLType as GraphQLNamedType)!.NameValue() == GraphQLToken.Float),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(exampleFloatValue);

        _mockGraphQLScalarToExampleValueConverter!
            .Convert(
                Arg.Is<GraphQLType>(graphQLType =>
                    (graphQLType as GraphQLNamedType)!.NameValue() == GraphQLToken.Boolean),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(exampleBooleanValue);

        const string expectedExampleValue =
            $"{{ \"exampleValue\": {exampleStringValue}, \"exampleValue2\": [ {exampleIntValue} ], \"exampleValue3\": {exampleFloatValue}, \"exampleValue4\": {exampleBooleanValue}, \"exampleValue5\": {{ \"nestedValue\": {exampleIntValue}, \"nestedValue2\": [ {exampleIntValue} ] }} }}";

        // act
        var exampleValue = _subjectUnderTest!.Convert(
            graphQLInputValueDefinition,
            _mockGraphQLDocumentAdapter
        );

        // assert
        exampleValue.Should().Be(expectedExampleValue);
    }

    [Test]
    public void Convert_returns_expected_example_value_for_recursive_input_type()
    {
        // arrange
        var nestedGraphQLInputObjectTypeDefinition = new GraphQLInputObjectTypeDefinition(
            new GraphQLName("TestNestedInputObjectTypeDefinition"))
        {
            Fields = new GraphQLInputFieldsDefinition(
                [
                    new GraphQLInputValueDefinition(
                        new GraphQLName("nestedValue"),
                        new GraphQLNamedType(new GraphQLName(GraphQLToken.Int))
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("recursiveNestedValue"),
                        new GraphQLNonNullType(
                            new GraphQLNamedType(new GraphQLName("TestInputObjectTypeDefinition"))
                        )
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("recursiveNestedListValue"),
                        new GraphQLListType(
                            new GraphQLNamedType(new GraphQLName("TestInputObjectTypeDefinition"))
                        )
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("recursiveNestedNonNullListValue"),
                        new GraphQLNonNullType(
                            new GraphQLListType(
                                new GraphQLNamedType(new GraphQLName("TestInputObjectTypeDefinition"))
                            )
                        )
                    )
                ]
            )
        };

        var graphQLInputObjectTypeDefinition = new GraphQLInputObjectTypeDefinition(
            new GraphQLName("TestInputObjectTypeDefinition"))
        {
            Fields = new GraphQLInputFieldsDefinition(
                [
                    new GraphQLInputValueDefinition(
                        new GraphQLName("exampleValue"),
                        new GraphQLNamedType(new GraphQLName(GraphQLToken.String))
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("exampleValue2"),
                        new GraphQLListType(
                            new GraphQLNamedType(new GraphQLName(GraphQLToken.Int))
                        )
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("exampleValue3"),
                        new GraphQLNamedType(new GraphQLName(GraphQLToken.Float))
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("exampleValue4"),
                        new GraphQLNamedType(new GraphQLName(GraphQLToken.Boolean))
                    ),

                    new GraphQLInputValueDefinition(
                        new GraphQLName("exampleValue5"),
                        new GraphQLNonNullType(
                            new GraphQLNamedType(new GraphQLName(nestedGraphQLInputObjectTypeDefinition.Name))
                        )
                    )
                ]
            )
        };

        var graphQLInputValueDefinition = new GraphQLInputValueDefinition(
            new GraphQLName("exampleInput"),
            new GraphQLNamedType(new GraphQLName(graphQLInputObjectTypeDefinition.NameValue()))
        );

        _mockGraphQLDocumentAdapter!
            .IsGraphQLInputObjectTypeDefinition(graphQLInputObjectTypeDefinition.NameValue())
            .Returns(true);

        _mockGraphQLDocumentAdapter!
            .IsGraphQLInputObjectTypeDefinition(nestedGraphQLInputObjectTypeDefinition.NameValue())
            .Returns(true);

        _mockGraphQLDocumentAdapter
            .GetGraphQLInputObjectTypeDefinition(graphQLInputObjectTypeDefinition.NameValue())
            .Returns(graphQLInputObjectTypeDefinition);

        _mockGraphQLDocumentAdapter
            .GetGraphQLInputObjectTypeDefinition(nestedGraphQLInputObjectTypeDefinition.NameValue())
            .Returns(nestedGraphQLInputObjectTypeDefinition);

        const string exampleStringValue = "\"an example value\"";
        const string exampleIntValue = "1";
        const string exampleFloatValue = "1.0";
        const string exampleBooleanValue = "true";

        _mockGraphQLScalarToExampleValueConverter!
            .Convert(
                Arg.Is<GraphQLType>(
                    graphQLType => (graphQLType as GraphQLNamedType)!.NameValue() == GraphQLToken.String),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(exampleStringValue);

        _mockGraphQLScalarToExampleValueConverter!
            .Convert(
                Arg.Is<GraphQLType>(graphQLType => (graphQLType as GraphQLNamedType)!.NameValue() == GraphQLToken.Int),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(exampleIntValue);

        _mockGraphQLScalarToExampleValueConverter!
            .Convert(
                Arg.Is<GraphQLType>(graphQLType =>
                    (graphQLType as GraphQLNamedType)!.NameValue() == GraphQLToken.Float),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(exampleFloatValue);

        _mockGraphQLScalarToExampleValueConverter!
            .Convert(
                Arg.Is<GraphQLType>(graphQLType =>
                    (graphQLType as GraphQLNamedType)!.NameValue() == GraphQLToken.Boolean),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(exampleBooleanValue);

        const string expectedExampleValue =
            $"{{ \"exampleValue\": {exampleStringValue}, \"exampleValue2\": [ {exampleIntValue} ], \"exampleValue3\": {exampleFloatValue}, \"exampleValue4\": {exampleBooleanValue}, \"exampleValue5\": {{ \"nestedValue\": {exampleIntValue}, \"recursiveNestedValue\": <some TestInputObjectTypeDefinition value>, \"recursiveNestedNonNullListValue\": [ <some TestInputObjectTypeDefinition value> ] }} }}";

        // act
        var exampleValue = _subjectUnderTest!.Convert(
            graphQLInputValueDefinition,
            _mockGraphQLDocumentAdapter
        );

        // assert
        exampleValue.Should().Be(expectedExampleValue);
    }

    [Test]
    public void Convert_throws_exception_when_unsupported_GraphQLType_is_encountered()
    {
        // arrange
        var graphQLInputValueDefinition = new GraphQLInputValueDefinition(
            new GraphQLName("exampleInput"),
            new UnsupportedGraphQLType()
        );

        _mockGraphQLDocumentAdapter!
            .IsGraphQLInputObjectTypeDefinition(Arg.Any<string>())
            .Returns(false);

        // act
        var act = () => _subjectUnderTest!.Convert(
            graphQLInputValueDefinition,
            _mockGraphQLDocumentAdapter
        );

        // assert
        act.Should().Throw<InvalidGraphQLTypeException>();
    }
}