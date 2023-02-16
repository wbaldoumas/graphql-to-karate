using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Parsers;
using GraphQLToKarate.Library.Settings;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLToKarateConverterTests
{
    private IGraphQLSchemaParser? _mockGraphQLSchemaParser;
    private IGraphQLTypeDefinitionConverter? _mockGraphQLTypeDefinitionConverter;
    private IGraphQLFieldDefinitionConverter? _mockGraphQLFieldDefinitionConverter;
    private IKarateFeatureBuilder? _mockKarateFeatureBuilder;

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLSchemaParser = Substitute.For<IGraphQLSchemaParser>();
        _mockGraphQLTypeDefinitionConverter = Substitute.For<IGraphQLTypeDefinitionConverter>();
        _mockGraphQLFieldDefinitionConverter = Substitute.For<IGraphQLFieldDefinitionConverter>();
        _mockKarateFeatureBuilder = Substitute.For<IKarateFeatureBuilder>();
    }

    [Test]
    public void Convert_generates_expected_karate_feature_and_invokes_expected_calls()
    {
        // arrange
        _mockGraphQLSchemaParser!
            .Parse(SomeSchemaString)
            .Returns(TestGraphQLDocument);

        _mockGraphQLTypeDefinitionConverter!
            .Convert(Arg.Any<GraphQLObjectTypeDefinition>(), Arg.Any<GraphQLDocumentAdapter>())
            .Returns(TestKarateObject);

        _mockGraphQLTypeDefinitionConverter!
            .Convert(Arg.Any<GraphQLInterfaceTypeDefinition>(), Arg.Any<GraphQLDocumentAdapter>())
            .Returns(OtherTestKarateObject);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.Name.StringValue == TodoQueryFieldDefinition.Name.StringValue
                ),
                Arg.Any<GraphQLDocumentAdapter>()
            )
            .Returns(TodoQueryFieldType);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.Name.StringValue == TodosQueryFieldDefinition.Name.StringValue
                ),
                Arg.Any<GraphQLDocumentAdapter>()
            )
            .Returns(TodosQueryFieldType);

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLQueryFieldType>>(),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(ExpectedKarateFeature)
            .AndDoes(ForceEnumerationOfMockedEnumerables);

        var settings = new GraphQLToKarateConverterSettings
        {
            QueryName = GraphQLToken.Query,
            ExcludeQueries = false,
            TypeFilter = new HashSet<string>()
        };

        var subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter,
            _mockKarateFeatureBuilder,
            settings
        );

        // act
        var karateFeature = subjectUnderTest.Convert(SomeSchemaString);

        // assert
        karateFeature.Should().Be(ExpectedKarateFeature);

        _mockGraphQLSchemaParser
            .Received(1)
            .Parse(SomeSchemaString);

        _mockGraphQLTypeDefinitionConverter
            .Received(1)
            .Convert(GraphQLObjectTypeDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLTypeDefinitionConverter
            .Received(1)
            .Convert(GraphQLInterfaceTypeDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodosQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 2),
                Arg.Is<IEnumerable<GraphQLQueryFieldType>>(arg => arg.Count() == 2),
                Arg.Any<IGraphQLDocumentAdapter>()
            );
    }

    [Test]
    public void Convert_only_invokes_GraphQLTypeDefinitionConverter_for_object_type_in_TypeFilter_setting()
    {
        // arrange
        _mockGraphQLSchemaParser!
            .Parse(SomeSchemaString)
            .Returns(TestGraphQLDocument);

        _mockGraphQLTypeDefinitionConverter!
            .Convert(Arg.Any<GraphQLObjectTypeDefinition>(), Arg.Any<GraphQLDocumentAdapter>())
            .Returns(TestKarateObject);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.Name.StringValue == TodoQueryFieldDefinition.Name.StringValue
                ),
                Arg.Any<GraphQLDocumentAdapter>()
            )
            .Returns(TodoQueryFieldType);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.Name.StringValue == TodosQueryFieldDefinition.Name.StringValue
                ),
                Arg.Any<GraphQLDocumentAdapter>()
            )
            .Returns(TodosQueryFieldType);

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLQueryFieldType>>(),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(ExpectedKarateFeature)
            .AndDoes(ForceEnumerationOfMockedEnumerables);

        var settings = new GraphQLToKarateConverterSettings
        {
            ExcludeQueries = false,
            QueryName = GraphQLToken.Query,
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                GraphQLObjectTypeDefinition.Name.StringValue
            }
        };

        var subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter,
            _mockKarateFeatureBuilder,
            settings
        );

        // act
        var karateFeature = subjectUnderTest.Convert(SomeSchemaString);

        // assert
        karateFeature.Should().Be(ExpectedKarateFeature);

        _mockGraphQLSchemaParser
            .Received(1)
            .Parse(SomeSchemaString);

        _mockGraphQLTypeDefinitionConverter
            .Received(1)
            .Convert(GraphQLObjectTypeDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLTypeDefinitionConverter
            .DidNotReceive()
            .Convert(GraphQLInterfaceTypeDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodosQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 1),
                Arg.Is<IEnumerable<GraphQLQueryFieldType>>(arg => arg.Count() == 2),
                Arg.Any<IGraphQLDocumentAdapter>()
            );
    }

    [Test]
    public void Convert_only_invokes_GraphQLTypeDefinitionConverter_for_interface_type_in_TypeFilter_setting()
    {
        // arrange
        _mockGraphQLSchemaParser!
            .Parse(SomeSchemaString)
            .Returns(TestGraphQLDocument);

        _mockGraphQLTypeDefinitionConverter!
            .Convert(Arg.Any<GraphQLInterfaceTypeDefinition>(), Arg.Any<GraphQLDocumentAdapter>())
            .Returns(TestKarateObject);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.Name.StringValue == TodoQueryFieldDefinition.Name.StringValue
                ),
                Arg.Any<GraphQLDocumentAdapter>()
            )
            .Returns(TodoQueryFieldType);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.Name.StringValue == TodosQueryFieldDefinition.Name.StringValue
                ),
                Arg.Any<GraphQLDocumentAdapter>()
            )
            .Returns(TodosQueryFieldType);

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLQueryFieldType>>(),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(ExpectedKarateFeature)
            .AndDoes(ForceEnumerationOfMockedEnumerables);

        var settings = new GraphQLToKarateConverterSettings
        {
            QueryName = GraphQLToken.Query,
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                GraphQLInterfaceTypeDefinition.Name.StringValue
            },
            ExcludeQueries = false
        };

        var subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter,
            _mockKarateFeatureBuilder,
            settings
        );

        // act
        var karateFeature = subjectUnderTest.Convert(SomeSchemaString);

        // assert
        karateFeature.Should().Be(ExpectedKarateFeature);

        _mockGraphQLSchemaParser
            .Received(1)
            .Parse(SomeSchemaString);

        _mockGraphQLTypeDefinitionConverter
            .Received(1)
            .Convert(GraphQLInterfaceTypeDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLTypeDefinitionConverter
            .DidNotReceive()
            .Convert(GraphQLObjectTypeDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodosQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>());

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 1),
                Arg.Is<IEnumerable<GraphQLQueryFieldType>>(arg => arg.Count() == 2),
                Arg.Any<IGraphQLDocumentAdapter>()
            );
    }

    private static void ForceEnumerationOfMockedEnumerables(CallInfo callInfo)
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
    }

    private const string SomeSchemaString = "some schema string";

    private const string ExpectedKarateFeature = "some feature string";

    private static readonly KarateObject TestKarateObject = new("TestKarateObject", new List<KarateTypeBase>());

    private static readonly KarateObject OtherTestKarateObject = new("OtherTestKarateObject", new List<KarateTypeBase>());

    private static readonly GraphQLFieldDefinition TodoQueryFieldDefinition = new()
    {
        Name = new GraphQLName("todo")
    };

    private static readonly GraphQLFieldDefinition TodosQueryFieldDefinition = new()
    {
        Name = new GraphQLName("todos")
    };

    private static readonly GraphQLObjectTypeDefinition GraphQLObjectTypeDefinition = new()
    {
        Name = new GraphQLName("TestGraphQLObjectTypeDefinition")
    };

    private static readonly GraphQLInterfaceTypeDefinition GraphQLInterfaceTypeDefinition = new()
    {
        Name = new GraphQLName("TestGraphQLInterfaceTypeDefinition")
    };

    private static readonly GraphQLEnumTypeDefinition GraphQLEnumTypeDefinition = new()
    {
        Name = new GraphQLName("TestGraphQLEnumTypeDefinition")
    };

    private static readonly GraphQLObjectTypeDefinition GraphQLQuery = new()
    {
        Name = new GraphQLName(GraphQLToken.Query),
        Fields = new GraphQLFieldsDefinition
        {
            Items = new List<GraphQLFieldDefinition>
            {
                TodoQueryFieldDefinition,
                TodosQueryFieldDefinition
            }
        }
    };

    private static readonly GraphQLDocument TestGraphQLDocument = new()
    {
        Definitions = new List<ASTNode>
        {
            GraphQLObjectTypeDefinition,
            GraphQLInterfaceTypeDefinition,
            GraphQLEnumTypeDefinition,
            GraphQLQuery
        }
    };

    private static readonly GraphQLQueryFieldType TodoQueryFieldType = new(TodoQueryFieldDefinition)
    {
        QueryString = "some query string",
        Arguments = new List<GraphQLArgumentTypeBase>()
    };

    private static readonly GraphQLQueryFieldType TodosQueryFieldType = new(TodosQueryFieldDefinition)
    {
        QueryString = "some other query string",
        Arguments = new List<GraphQLArgumentTypeBase>()
    };
}