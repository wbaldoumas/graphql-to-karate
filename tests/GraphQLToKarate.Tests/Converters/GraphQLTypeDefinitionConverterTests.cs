using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQlTypeDefinitionConverterTests
{
    private IGraphQLTypeConverterFactory? _mockGraphQLTypeConverterFactory;
    private IGraphQLTypeConverter? _mockNonNullGraphQLTypeConverter;
    private IGraphQLTypeConverter? _mockNullGraphQLTypeConverter;
    private IGraphQLDocumentAdapter? _mockGraphQLDocumentAdapter;
    private IGraphQLTypeDefinitionConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLTypeConverterFactory = Substitute.For<IGraphQLTypeConverterFactory>();
        _mockNonNullGraphQLTypeConverter = Substitute.For<IGraphQLTypeConverter>();
        _mockNullGraphQLTypeConverter = Substitute.For<IGraphQLTypeConverter>();

        _mockGraphQLTypeConverterFactory
            .CreateGraphQLNonNullTypeConverter()
            .Returns(_mockNonNullGraphQLTypeConverter);

        _mockGraphQLTypeConverterFactory
            .CreateGraphQLNullTypeConverter()
            .Returns(_mockNullGraphQLTypeConverter);

        _mockGraphQLDocumentAdapter = Substitute.For<IGraphQLDocumentAdapter>();

        _subjectUnderTest = new GraphQLTypeDefinitionConverter(_mockGraphQLTypeConverterFactory);
    }

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Convert(
        GraphQLObjectTypeDefinition graphQLObjectTypeDefinition,
        IDictionary<string, KarateTypeBase> karateTypesByFieldDefinitionName)
    {
        // arrange
        foreach (var (fieldDefinitionName, karateType) in karateTypesByFieldDefinitionName)
        {
            if (karateType is KarateNonNullType)
            {
                _mockNonNullGraphQLTypeConverter!
                    .Convert(
                        Arg.Is<string>(arg => arg == fieldDefinitionName),
                        Arg.Any<GraphQLType>(),
                        Arg.Any<IGraphQLDocumentAdapter>()
                    )
                    .Returns(karateType);
            }
            else
            {
                _mockNullGraphQLTypeConverter!
                    .Convert(
                        Arg.Is<string>(arg => arg == fieldDefinitionName),
                        Arg.Any<GraphQLType>(),
                        Arg.Any<IGraphQLDocumentAdapter>()
                    )
                    .Returns(karateType);
            }
        }

        var expectedKarateObject = new KarateObject(
            graphQLObjectTypeDefinition.Name.StringValue,
            karateTypesByFieldDefinitionName.Values.ToList()
        );

        // act
        var karateObject = _subjectUnderTest!.Convert(
            graphQLObjectTypeDefinition,
            _mockGraphQLDocumentAdapter!
        );

        // assert
        karateObject
            .Should()
            .BeEquivalentTo(expectedKarateObject);
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            yield return new TestCaseData(
                new GraphQLObjectTypeDefinition
                {
                    Name = new GraphQLName("Test Object GraphQL Type"),
                    Fields = new GraphQLFieldsDefinition
                    {
                        Items = new List<GraphQLFieldDefinition>
                        {
                            NonNullGraphQLFieldDefinition,
                            NullGraphQLFieldDefinition,
                            ListGraphQLFieldDefinition
                        }
                    }
                },
                new Dictionary<string, KarateTypeBase>
                {
                    {
                        NonNullGraphQLFieldDefinition.Name.StringValue,
                        new KarateNonNullType(
                            new KarateType(
                                KarateToken.Number,
                                NonNullGraphQLFieldDefinition.Name.StringValue
                            )
                        )
                    },
                    {
                        NullGraphQLFieldDefinition.Name.StringValue,
                        new KarateNullType(
                            new KarateType(
                                KarateToken.String,
                                NullGraphQLFieldDefinition.Name.StringValue
                            )
                        )
                    },
                    {
                        ListGraphQLFieldDefinition.Name.StringValue,
                        new KarateListType(
                            new KarateType(
                                KarateToken.Boolean,
                                ListGraphQLFieldDefinition.Name.StringValue
                            )
                        )
                    }
                }
            ).SetName("Each GraphQLType is handled as expected and generates expected Karate object.");

            yield return new TestCaseData(
                new GraphQLObjectTypeDefinition
                {
                    Name = new GraphQLName("Test Object GraphQL Type")
                },
                new Dictionary<string, KarateTypeBase>()
            ).SetName("Empty GraphQL object generates empty Karate object.");
        }
    }

    private static readonly GraphQLFieldDefinition NonNullGraphQLFieldDefinition = new()
    {
        Name = new GraphQLName("non_nullable_field"),
        Type = new GraphQLNonNullType()
    };

    private static readonly GraphQLFieldDefinition ListGraphQLFieldDefinition = new()
    {
        Name = new GraphQLName("list_field"),
        Type = new GraphQLListType()
    };

    private static readonly GraphQLFieldDefinition NullGraphQLFieldDefinition = new()
    {
        Name = new GraphQLName("nullable_field"),
        Type = new GraphQLNamedType()
    };
}