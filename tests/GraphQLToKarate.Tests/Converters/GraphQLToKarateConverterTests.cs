using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Enums;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Features;
using GraphQLToKarate.Library.Parsers;
using GraphQLToKarate.Library.Settings;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using Microsoft.Extensions.Logging;
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
    private ILogger<GraphQLToKarateConverter>? _mockLogger;

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLSchemaParser = Substitute.For<IGraphQLSchemaParser>();
        _mockGraphQLTypeDefinitionConverter = Substitute.For<IGraphQLTypeDefinitionConverter>();
        _mockGraphQLFieldDefinitionConverter = Substitute.For<IGraphQLFieldDefinitionConverter>();
        _mockKarateFeatureBuilder = Substitute.For<IKarateFeatureBuilder>();
        _mockLogger = Substitute.For<ILogger<GraphQLToKarateConverter>>();
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
                    arg => arg.NameValue() == TodoQueryFieldDefinition.NameValue()
                ),
                Arg.Any<GraphQLDocumentAdapter>(),
                Arg.Any<GraphQLOperationType>()
            )
            .Returns(TodoOperation);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.NameValue() == TodosQueryFieldDefinition.NameValue()
                ),
                Arg.Any<GraphQLDocumentAdapter>(),
                Arg.Any<GraphQLOperationType>()
            )
            .Returns(TodosOperation);

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLOperation>>(),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(ExpectedKarateFeature)
            .AndDoes(ForceEnumerationOfMockedEnumerables);

        var settings = new GraphQLToKarateSettings
        {
            QueryName = GraphQLToken.Query,
            ExcludeQueries = false,
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        };

        var subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter,
            _mockKarateFeatureBuilder,
            _mockLogger!,
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
            .Convert(TodoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodosQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 2),
                Arg.Is<IEnumerable<GraphQLOperation>>(arg => arg.Count() == 2),
                Arg.Any<IGraphQLDocumentAdapter>()
            );
    }

    [Test]
    public void Convert_generates_expected_karate_feature_and_invokes_expected_calls_with_mutations_enabled()
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
                    arg => arg.NameValue() == TodoQueryFieldDefinition.NameValue()
                ),
                Arg.Any<GraphQLDocumentAdapter>(),
                Arg.Any<GraphQLOperationType>()
            )
            .Returns(TodoOperation);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.NameValue() == TodosQueryFieldDefinition.NameValue()
                ),
                Arg.Any<GraphQLDocumentAdapter>(),
                Arg.Any<GraphQLOperationType>()
            )
            .Returns(TodosOperation);

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLOperation>>(),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(ExpectedKarateFeature)
            .AndDoes(ForceEnumerationOfMockedEnumerables);

        var settings = new GraphQLToKarateSettings
        {
            QueryName = GraphQLToken.Query,
            ExcludeQueries = false,
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            IncludeMutations = true
        };

        var subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter,
            _mockKarateFeatureBuilder,
            _mockLogger!,
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
            .Convert(TodoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), GraphQLOperationType.Query);

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodosQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), GraphQLOperationType.Query);

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodoMutationFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), GraphQLOperationType.Mutation);

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodosMutationFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), GraphQLOperationType.Mutation);

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 2),
                Arg.Is<IEnumerable<GraphQLOperation>>(arg => arg.Count() == 4),
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
                    arg => arg.NameValue() == TodoQueryFieldDefinition.NameValue()
                ),
                Arg.Any<GraphQLDocumentAdapter>(),
                Arg.Any<GraphQLOperationType>()
            )
            .Returns(TodoOperation);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.NameValue() == TodosQueryFieldDefinition.NameValue()
                ),
                Arg.Any<GraphQLDocumentAdapter>(),
                Arg.Any<GraphQLOperationType>()
            )
            .Returns(TodosOperation);

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLOperation>>(),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(ExpectedKarateFeature)
            .AndDoes(ForceEnumerationOfMockedEnumerables);

        var settings = new GraphQLToKarateSettings
        {
            ExcludeQueries = false,
            QueryName = GraphQLToken.Query,
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                GraphQLObjectTypeDefinition.NameValue()
            }
        };

        var subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter,
            _mockKarateFeatureBuilder,
            _mockLogger!,
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
            .Convert(TodoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodosQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 1),
                Arg.Is<IEnumerable<GraphQLOperation>>(arg => arg.Count() == 2),
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
                    arg => arg.NameValue() == TodoQueryFieldDefinition.NameValue()
                ),
                Arg.Any<GraphQLDocumentAdapter>(),
                Arg.Any<GraphQLOperationType>()
            )
            .Returns(TodoOperation);

        _mockGraphQLFieldDefinitionConverter!
            .Convert(
                Arg.Is<GraphQLFieldDefinition>(
                    arg => arg.NameValue() == TodosQueryFieldDefinition.NameValue()
                ),
                Arg.Any<GraphQLDocumentAdapter>(),
                Arg.Any<GraphQLOperationType>()
            )
            .Returns(TodosOperation);

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLOperation>>(),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(ExpectedKarateFeature)
            .AndDoes(ForceEnumerationOfMockedEnumerables);

        var settings = new GraphQLToKarateSettings
        {
            QueryName = GraphQLToken.Query,
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                GraphQLInterfaceTypeDefinition.NameValue()
            },
            ExcludeQueries = false
        };

        var subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter,
            _mockKarateFeatureBuilder,
            _mockLogger!,
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
            .Convert(TodoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockGraphQLFieldDefinitionConverter
            .Received(1)
            .Convert(TodosQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 1),
                Arg.Is<IEnumerable<GraphQLOperation>>(arg => arg.Count() == 2),
                Arg.Any<IGraphQLDocumentAdapter>()
            );
    }

    [Test]
    public void Convert_only_invokes_GraphQLFieldDefinitionConverter_for_Todo_query_in_OperationFilter_setting()
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
                    arg => arg.NameValue() == TodoQueryFieldDefinition.NameValue()
                ),
                Arg.Any<GraphQLDocumentAdapter>(),
                Arg.Any<GraphQLOperationType>()
            )
            .Returns(TodoOperation);

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLOperation>>(),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(ExpectedKarateFeature)
            .AndDoes(ForceEnumerationOfMockedEnumerables);

        var settings = new GraphQLToKarateSettings
        {
            QueryName = GraphQLToken.Query,
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            OperationFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                TodoQueryFieldDefinition.NameValue()
            },
            ExcludeQueries = false
        };

        var subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter,
            _mockKarateFeatureBuilder,
            _mockLogger!,
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
            .Convert(TodoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockGraphQLFieldDefinitionConverter
            .DidNotReceive()
            .Convert(TodosQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 2),
                Arg.Is<IEnumerable<GraphQLOperation>>(arg => arg.Count() == 1),
                Arg.Any<IGraphQLDocumentAdapter>()
            );
    }

    [Test]
    public void Convert_only_invokes_GraphQLFieldDefinitionConverter_for_Todos_query_in_OperationFilter_setting()
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
                    arg => arg.NameValue() == TodosQueryFieldDefinition.NameValue()
                ),
                Arg.Any<GraphQLDocumentAdapter>(),
                Arg.Any<GraphQLOperationType>()
            )
            .Returns(TodosOperation);

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLOperation>>(),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(ExpectedKarateFeature)
            .AndDoes(ForceEnumerationOfMockedEnumerables);

        var settings = new GraphQLToKarateSettings
        {
            QueryName = GraphQLToken.Query,
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            OperationFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                TodosQueryFieldDefinition.NameValue()
            },
            ExcludeQueries = false
        };

        var subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter,
            _mockKarateFeatureBuilder,
            _mockLogger!,
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
            .DidNotReceive()
            .Convert(TodoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 2),
                Arg.Is<IEnumerable<GraphQLOperation>>(arg => arg.Count() == 1),
                Arg.Any<IGraphQLDocumentAdapter>()
            );
    }

    [Test]
    public void Convert_skips_scenario_generation_and_logs_warning_when_query_not_found()
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

        _mockKarateFeatureBuilder!
            .Build(
                Arg.Any<IEnumerable<KarateObject>>(),
                Arg.Any<IEnumerable<GraphQLOperation>>(),
                Arg.Any<IGraphQLDocumentAdapter>()
            )
            .Returns(ExpectedKarateFeature)
            .AndDoes(ForceEnumerationOfMockedEnumerables);

        var settings = new GraphQLToKarateSettings
        {
            QueryName = "SomeWackyQueryName",
            TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            OperationFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ExcludeQueries = false,
            MutationName = "SomeWackyMutationName",
            IncludeMutations = true
        };

        var subjectUnderTest = new GraphQLToKarateConverter(
            _mockGraphQLSchemaParser,
            _mockGraphQLTypeDefinitionConverter,
            _mockGraphQLFieldDefinitionConverter!,
            _mockKarateFeatureBuilder,
            _mockLogger!,
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

        _mockGraphQLFieldDefinitionConverter!
            .DidNotReceive()
            .Convert(TodoQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockGraphQLFieldDefinitionConverter!
            .DidNotReceive()
            .Convert(TodosQueryFieldDefinition, Arg.Any<GraphQLDocumentAdapter>(), Arg.Any<GraphQLOperationType>());

        _mockKarateFeatureBuilder
            .Received(1)
            .Build(
                Arg.Is<IEnumerable<KarateObject>>(arg => arg.Count() == 4),
                Arg.Is<IEnumerable<GraphQLOperation>>(arg => !arg.Any()),
                Arg.Any<IGraphQLDocumentAdapter>()
            );

        // LogWarning is an extension method, so we need to hack around verifying that it was called...
        _mockLogger
            .ReceivedCalls()
            .Select(call => call.GetArguments())
            .Count(callArguments => ((LogLevel)callArguments[0]!).Equals(LogLevel.Warning))
            .Should()
            .Be(2);
    }

    private static void ForceEnumerationOfMockedEnumerables(CallInfo callInfo)
    {
        var karateObjects = callInfo.ArgAt<IEnumerable<KarateObject>>(0);
        var graphQLQueries = callInfo.ArgAt<IEnumerable<GraphQLOperation>>(1);

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

    private static readonly KarateObject OtherTestKarateObject =
        new("OtherTestKarateObject", new List<KarateTypeBase>());

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

    private static readonly GraphQLFieldDefinition TodosMutationFieldDefinition = new()
    {
        Name = new GraphQLName("todosMutation")
    };

    private static readonly GraphQLFieldDefinition TodoMutationFieldDefinition = new()
    {
        Name = new GraphQLName("todoMutation")
    };

    private static readonly GraphQLObjectTypeDefinition GraphQLMutation = new()
    {
        Name = new GraphQLName(GraphQLToken.Mutation),
        Fields = new GraphQLFieldsDefinition
        {
            Items = new List<GraphQLFieldDefinition>
            {
                TodosMutationFieldDefinition,
                TodoMutationFieldDefinition
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
            GraphQLQuery,
            GraphQLMutation
        }
    };

    private static readonly GraphQLOperation TodoOperation = new(TodoQueryFieldDefinition)
    {
        OperationString = "some query string",
        Arguments = new List<GraphQLArgumentTypeBase>(),
        Type = GraphQLOperationType.Query
    };

    private static readonly GraphQLOperation TodosOperation = new(TodosQueryFieldDefinition)
    {
        OperationString = "some other query string",
        Arguments = new List<GraphQLArgumentTypeBase>(),
        Type = GraphQLOperationType.Query
    };
}