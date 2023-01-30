using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Parsers;
using GraphQLToKarate.Library.Types;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLToKarateConverterTests
{
    private IGraphQLSchemaParser? _mockGraphQLSchemaParser;
    private IGraphQLTypeDefinitionConverter? _mockGraphQLTypeDefinitionConverter;
    private IGraphQLFieldDefinitionConverter? _mockGraphQLFieldDefinitionConverter;
    private IKarateFeatureBuilder? _mockKarateFeatureBuilder;
    private IGraphQLToKarateConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLSchemaParser = Substitute.For<IGraphQLSchemaParser>();
        _mockGraphQLTypeDefinitionConverter = Substitute.For<IGraphQLTypeDefinitionConverter>();
        _mockGraphQLFieldDefinitionConverter = Substitute.For<IGraphQLFieldDefinitionConverter>();
        _mockKarateFeatureBuilder = Substitute.For<IKarateFeatureBuilder>();

        _subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter,
            _mockKarateFeatureBuilder
        );
    }

    [Test]
    public void Convert_generates_expected_karate_feature_and_invokes_expected_calls()
    {
        // arrange
        var todoQueryFieldDefinition = new GraphQLFieldDefinition
        {
            Name = new GraphQLName("todo")
        };

        var todosQueryFieldDefinition = new GraphQLFieldDefinition
        {
            Name = new GraphQLName("todos")
        };

        var graphQLObjectTypeDefinition = new GraphQLObjectTypeDefinition
        {
            Name = new GraphQLName("TestGraphQLObjectTypeDefinition")
        };

        var graphQLInterfaceTypeDefinition = new GraphQLInterfaceTypeDefinition
        {
            Name = new GraphQLName("TestGraphQLInterfaceTypeDefinition")
        };

        var graphQLEnumTypeDefinition = new GraphQLEnumTypeDefinition
        {
            Name = new GraphQLName("TestGraphQLEnumTypeDefinition")
        };

        var graphQLQuery = new GraphQLObjectTypeDefinition
        {
            Name = new GraphQLName("Query"),
            Fields = new GraphQLFieldsDefinition
            {
                Items = new List<GraphQLFieldDefinition>
                {
                    todoQueryFieldDefinition,
                    todosQueryFieldDefinition
                }
            }
        };

        var testGraphQLDocument = new GraphQLDocument
        {
            Definitions = new List<ASTNode>
            {
                graphQLObjectTypeDefinition,
                graphQLInterfaceTypeDefinition,
                graphQLEnumTypeDefinition,
                graphQLQuery
            }
        };

        const string someSchemaString = "some schema string";

        _mockGraphQLSchemaParser!
            .Parse(someSchemaString)
            .Returns(testGraphQLDocument);

        var testKarateObject = new KarateObject("TestKarateObject", new List<KarateTypeBase>());
        var otherTestKarateObject = new KarateObject("OtherTestKarateObject", new List<KarateTypeBase>());

        _mockGraphQLTypeDefinitionConverter!
            .Convert(Arg.Any<GraphQLObjectTypeDefinition>(), Arg.Any<GraphQLDocumentAdapter>())
            .Returns(testKarateObject);

        _mockGraphQLTypeDefinitionConverter!
            .Convert(Arg.Any<GraphQLInterfaceTypeDefinition>(), Arg.Any<GraphQLDocumentAdapter>())
            .Returns(otherTestKarateObject);

        var todoQueryFieldType = new GraphQLQueryFieldType(todoQueryFieldDefinition)
        {
            QueryString = "some query string",
            Arguments = new List<GraphQLArgumentTypeBase>()
        };

        var todosQueryFieldType = new GraphQLQueryFieldType(todosQueryFieldDefinition)
        {
            QueryString = "some other query string",
            Arguments = new List<GraphQLArgumentTypeBase>()
        };

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.Name.StringValue == todoQueryFieldDefinition.Name.StringValue
                ),
                Arg.Any<GraphQLDocumentAdapter>()
            )
            .Returns(todoQueryFieldType);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.Name.StringValue == todosQueryFieldDefinition.Name.StringValue
                ),
                Arg.Any<GraphQLDocumentAdapter>()
            )
            .Returns(todosQueryFieldType);

        const string expectedKarateFeature = "some feature string";

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLQueryFieldType>>()
            )
            .Returns(expectedKarateFeature)
            .AndDoes(callInfo =>
            {
                var karateObjects = callInfo.ArgAt<IEnumerable<KarateObject>>(0);
                var graphQLQueries = callInfo.ArgAt<IEnumerable<GraphQLQueryFieldType>>(1);

                foreach (var _ in karateObjects)
                {
                    // force enumeration to enable checking received calls below
                }

                foreach (var _ in graphQLQueries)
                {
                    // force enumeration to enable checking received calls below
                }
            });

        // act
        var karateFeature = _subjectUnderTest!.Convert(someSchemaString);

        // assert
        karateFeature.Should().Be(expectedKarateFeature);

        _mockGraphQLSchemaParser
            .Received(1)
            .Parse(someSchemaString);

        _mockGraphQLTypeDefinitionConverter
            .Received(1)
            .Convert(graphQLObjectTypeDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLTypeDefinitionConverter
            .Received(1)
            .Convert(graphQLInterfaceTypeDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(todoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(todosQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 2),
                Arg.Is<IEnumerable<GraphQLQueryFieldType>>(arg => arg.Count() == 2)
            );
    }
}